using DataverseRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

#if MOCKUP
using DG.Tools.XrmMockup;
#endif


namespace API.DataverseAccess
{
    public class DVDataAccessFactory
    {
        private IOrganizationService dvService = null;

        private ConnectionConfiguration connectionConfiguration = null;
        private IMemoryCache cache = null;
        private string overridenImpersonationEmailAddress;
        private IHttpContextAccessor httpContextAccessor;

#if MOCKUP
        private XrmMockupDataverse crm;
#endif

        public DVDataAccessFactory(ConnectionConfiguration connectionConfiguration, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.connectionConfiguration = connectionConfiguration;
            this.cache = cache;
        }
        public DVDataAccessFactory(ConnectionConfiguration connectionConfiguration, IMemoryCache cache, string emailAddress)
        {
            overridenImpersonationEmailAddress = emailAddress;
            this.connectionConfiguration = connectionConfiguration;
            this.cache = cache;
        }

        public string ImpersonationEmailAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(overridenImpersonationEmailAddress))
                {
                    return overridenImpersonationEmailAddress;
                }
                else
                {
                    return httpContextAccessor.HttpContext.Request.Headers["UserEmail"];
                }
            }
        }

        private Guid GetUserADObjectId(string emailAddress)
        {
            Guid adId;
            if (!cache.TryGetValue($"{emailAddress}:AzureActiveDirectoryObjectId", out adId))
            {
                QueryExpression query = new QueryExpression("systemuser");
                query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, emailAddress);
                query.ColumnSet = new ColumnSet("azureactivedirectoryobjectid");

                var dvResponse = AdminDvService.RetrieveMultiple(query);

                adId = dvResponse.Entities.Single().GetAttributeValue<Guid>("azureactivedirectoryobjectid");

                cache.Set($"{emailAddress}:AzureActiveDirectoryObjectId", adId);
            }

            return adId;
        }

        private Guid GetUserId(string emailAddress)
        {
            Guid userId;
            if (!cache.TryGetValue($"{emailAddress}:UserId", out userId))
            {
                QueryExpression query = new QueryExpression("systemuser");
                query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, emailAddress);
                
                var dvResponse = AdminDvService.RetrieveMultiple(query);

                userId = dvResponse.Entities.Single().Id;

                cache.Set($"{emailAddress}:UserId", userId);
            }

            return userId;
        }

        public IOrganizationService AdminDvService
        {
            get
            {
#if !MOCKUP
              //  if (dvService == null || !(dvService as ServiceClient).IsReady)
                //{
                    Uri uri = new Uri(connectionConfiguration.DVUrl);
                    return new ServiceClient(uri, connectionConfiguration.ClientId, connectionConfiguration.ClientSecret, true);
                //}
            
#else
                if (dvService == null)
                {
                    var settings = new XrmMockupSettings
                    {
                        BasePluginTypes = new Type[] { },
                        CodeActivityInstanceTypes = new Type[] { },
                        EnableProxyTypes = false,
                        IncludeAllWorkflows = true,
                        AppendAndAppendToPrivilegeCheck = true,
                        MetadataDirectoryPath = @"C:\dev\PowerAppsForPolicing\MiddleTier\Tests\APITests\Metadata\"
                    };

                    crm = XrmMockupDataverse.GetInstance(settings);

                    dvService = crm.GetAdminService();

                    var user = new Entity("systemuser");
                    user.Id = Guid.Parse("eb976487-ccbd-4594-925a-c6a092b4a0e6");
                    user["internalemailaddress"] = "matt.trinder@tisski.com";
                    user["azureactivedirectoryobjectid"] = Guid.NewGuid();

                    dvService.Create(user);
                }
#endif
              //  return dvService;

            }
        }

        public DVDataAccess GetAdminDataAccess()
        {
            return new DVDataAccess(AdminDvService, cache);
        }

        public DVDataAccess GetUserDataAccess()
        {
            IOrganizationService userServiceClient = null;

            Uri uri = new Uri(connectionConfiguration.DVUrl);

#if MOCKUP
            var impersonationId = GetUserADObjectId(ImpersonationEmailAddress);
            userServiceClient = crm.CreateOrganizationService(impersonationId);
#else
            userServiceClient = new ServiceClient(uri, connectionConfiguration.ClientId, connectionConfiguration.ClientSecret, true);
            (userServiceClient as ServiceClient).CallerAADObjectId = GetUserADObjectId(ImpersonationEmailAddress);
#endif
            
            return new DVDataAccess(userServiceClient, cache, GetUserId(ImpersonationEmailAddress));
        }
    }
}