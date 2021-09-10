using DataverseRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace API.DataverseAccess
{
    public class DVDataAccessFactory
    {
        //ASSUMPTIONS:
        //* ServiceClient is thread safe.
        //* ServiceClient implements its own connection caching internally.
        //  So invoking 'new ServiceClient' does not mean a new connection will be created unless 'useUniqueInstance' is set to true.
        //* Creating a ServiceClient with useUniqueInstance (true) is much more time consuming as it forces reauthentication.
        //* So overriding ServiceClient's caching (by useUniqueInstance) or thread safety (by DisableCrossThreadSafeties) should not be 
        //  required aside from exceptional circumstances which should not apply to this API.
        //* User context is achieved by setting CallerAADObjectId on a ServiceClient instance.
        //  As ServiceClient should be thread safe, it may be possible to set this inline (within GetUserDataAccess) on a single ServiceClient 
        //  instance shared across the factory/API/threads but documentation is not clear on this so to be explicit/safe, we create a ServiceClient
        //  instance per CallerAADObjectId.
        //  (Remembering that due to internal caching, ServiceClient may be reusing a connection anyway even though we are creating 
        //   individual ServiceClient instances to hold each CallerAADObjectId.)
        //* Documentation is also not clear on how long a ServiceClient instance is valid for or likewise if there is significant overhead in 
        //  always instantiating new ServiceClient instances (even when useUniqueInstance is false) so again to be explicit/safe we cache each 
        //  ServiceClient instance for a specific amount of time.
        private ServiceClient dvService = null;

        private const int userDvServiceCacheSlidingExpirationMinutes = 240;
        private const int userDvServiceCacheAbsoluteExpirationMinutes = 720;

        private IHttpContextAccessor httpContextAccessor = null;
        private ConnectionConfiguration connectionConfiguration = null;
        private IMemoryCache cache = null;        

        public DVDataAccessFactory(ConnectionConfiguration connectionConfiguration, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.connectionConfiguration = connectionConfiguration;
            this.cache = cache;
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

        public ServiceClient AdminDvService
        {
            get
            {
                if (dvService == null || !dvService.IsReady)
                {
                    Uri uri = new Uri(connectionConfiguration.DVUrl);
                    dvService = new ServiceClient(uri, connectionConfiguration.ClientId, connectionConfiguration.ClientSecret, false);
                }
                return dvService;
            }
        }

        public DVDataAccess GetAdminDataAccess()
        {
            return new DVDataAccess(AdminDvService, cache);
        }

        public DVDataAccess GetUserDataAccess()
        {
            string emailAddress = httpContextAccessor.HttpContext.Request.Headers["UserEmail"];

            bool requireNewServiceClient = true;
            ServiceClient userServiceClient = null;
            if (cache.TryGetValue($"{emailAddress}:ServiceClient", out userServiceClient))
            {
                requireNewServiceClient = !userServiceClient.IsReady;
            }

            if (requireNewServiceClient)
            {
                Uri uri = new Uri(connectionConfiguration.DVUrl);
                userServiceClient = new ServiceClient(uri, connectionConfiguration.ClientId, connectionConfiguration.ClientSecret, false);
                userServiceClient.CallerAADObjectId = GetUserADObjectId(emailAddress);
                
                cache.Set($"{emailAddress}:ServiceClient", userServiceClient, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(userDvServiceCacheSlidingExpirationMinutes),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(userDvServiceCacheAbsoluteExpirationMinutes)
                });
            }
            
            return new DVDataAccess(userServiceClient, cache, GetUserId(emailAddress));
        }
    }
}