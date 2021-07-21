using API.Helpers;
using API.Models.Base;
using API.Models.IYC;
using Common.Models.Business;
using Common.Models.Dataverse;
using DataverseRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SystemTextJsonSamples;

namespace API.DataverseAccess
{
    public class DVDataAccess
    {
        private string _dvUrl;
        private string _serviceUrl;
        private string _clientId;
        private string _clientSecret;
        private string _authority;
        private IMemoryCache _cache;
        private Guid? _contextUserADObjectId;

        public DVDataAccess(ConnectionConfiguration config,IMemoryCache cache,Guid contexUserADObjectId)
        {
            _dvUrl = config.DVUrl;
            _serviceUrl = config.ServiceUrl;
            _clientId = config.ClientId;
            _clientSecret = config.ClientSecret;
            _authority = config.Authority;
            _cache = cache;
            _contextUserADObjectId = contexUserADObjectId;

        }

        public DVDataAccess(ConnectionConfiguration config, IMemoryCache cache)
        {
            _dvUrl = config.DVUrl;
            _serviceUrl = config.ServiceUrl;
            _clientId = config.ClientId;
            _clientSecret = config.ClientSecret;
            _authority = config.Authority;
            _cache = cache;


            _contextUserADObjectId = null;
        }

        public async Task<string> GetAuthToken()
        {

            var authContext = new AuthenticationContext(_authority);
            ClientCredential credential = new ClientCredential(_clientId, _clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(_serviceUrl, credential);
            return result.AccessToken;

        }

        public Guid GetUserId(string emailAddress)
        {
            Guid userId;
            if (!_cache.TryGetValue($"{emailAddress}:UserId", out userId))
            {
                var client = GetRestClient();
                var req = GetRestRequest($"systemusers?$filter=internalemailaddress eq '{emailAddress}'&$select=systemuserid", Method.GET);
                var response = client.Execute<DVGETResponse<DVSystemUser>>(req);

                _cache.Set($"{emailAddress}:UserId", response.Data.value.Single().SystemUserId);
                return response.Data.value.Single().SystemUserId;
            }

            return userId;

        }
        public Guid GetUserADObjectId(string emailAddress)
        {
            Guid userId;
            if (!_cache.TryGetValue($"{emailAddress}:AzureActiveDirectoryObjectId", out userId))
            {
                var client = GetRestClient();
                var req = GetRestRequest($"systemusers?$filter=internalemailaddress eq '{emailAddress}'&$select=azureactivedirectoryobjectid", Method.GET);
                var response = client.Execute<DVGETResponse<DVSystemUser>>(req);

                _cache.Set($"{emailAddress}:AzureActiveDirectoryObjectId", response.Data.value.Single().AzureActiveDirectoryObjectId);
                return response.Data.value.Single().AzureActiveDirectoryObjectId;
            }

            return userId;

        }

        private static string GetEntityPluralName(string entityName)
        {
            //todo exceptions to the general rule here

            return entityName + "s";
        }

        public ICollection<T> GetAll<T>()
        {
            return GetAll<T>(null, null);
        }

        public ICollection<T> GetAll<T>(string filter)
        {
            return GetAll<T>(filter, null);
        }

        private string GetEntityName<T>()
        {
            var attrs = typeof(T).GetCustomAttributes(true);
            var dataContractAttr = attrs.SingleOrDefault(x => x.GetType() == typeof(DataContractAttribute));
            return (dataContractAttr as DataContractAttribute).Name;
        }
        private string GetEntityName(DVBase entity)
        {
            var attrs = entity.GetType().GetCustomAttributes(true);
            var dataContractAttr = attrs.SingleOrDefault(x => x.GetType() == typeof(DataContractAttribute));
            return (dataContractAttr as DataContractAttribute).Name;
        }

        public T GetEntityByField<T>(string field,string value)
        {

            var client = GetRestClient();
            var req = GetRestRequest($"{GetEntityPluralName(GetEntityName<T>())}?$filter={field} eq '{value}'", Method.GET);
            var resp = client.Execute<DVGETResponse<T>>(req);

            //PopulateEntityReferences<T>(resp.Data.value);

            return resp.Data.value.SingleOrDefault();

        }

        public ICollection<T> GetAll<T>(string filter, string orderby)
        {
            var entityName = GetEntityName<T>();

            var client = GetRestClient();

            List<string> queryParts = new List<string>();
            if (!string.IsNullOrEmpty(filter))
            {
                queryParts.Add($"$filter={filter}");
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                queryParts.Add($"$orderby={orderby}");
            }

            string query = $"{GetEntityPluralName(entityName)}";
            if (queryParts.Any())
            {
                query += "?" + string.Join("&", queryParts);
            }

            var req = GetRestRequest(query, Method.GET);
            var resp = client.Execute<DVGETResponse<T>>(req);

            PopulateEntityReferences(resp.Data.value);

            return resp.Data.value;
        }


  
        private static void PopulateEntityReferences<T>(ICollection<T> collection)
        {
            foreach (var ent in collection)
            {
                PopulateEntityReferences(ent);
            }
        }

        private static void PopulateEntityReferences<T>(T ent)
        {
            foreach (var prop in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(EntityReference)))
            {
                var er = prop.GetValue(ent);
                if (er is null)
                {
                    foreach (var attr in prop.GetCustomAttributes().Where(x => x.GetType() == typeof(RelatedEntityNameAttribute)))
                    {
                        //find the matching property
                        var matchingProp = ent.GetType().GetProperty($"_{prop.Name}_value");
                        if (matchingProp != null)
                        {
                            er = new EntityReference((attr as RelatedEntityNameAttribute).Name, (Guid)matchingProp.GetValue(ent));
                            prop.SetValue(ent, er);
                        }
                    }
                }
            }
          
        }

        private RestClient GetRestClient()
        {
            RestClient client;

            client = new RestClient(_dvUrl);
            client.AddDefaultHeader("Authorization", "Bearer " + GetAuthToken().Result);
            if (_contextUserADObjectId.HasValue)
            {
                client.AddDefaultHeader("CallerObjectId", _contextUserADObjectId.ToString());
            }
            

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new LookupJsonConverter());
            client.UseSystemTextJson(options);

            return client;
        }


        private RestRequest GetRestRequest(string url, Method method)
        {
            var req = new RestRequest(url, method);// { RootElement = "value" };

            //req.JsonSerializer = new IntegrationService.JsonSerializer();

            if (method == Method.POST || method == Method.PATCH) //adding or updating
            {
                req.RequestFormat = DataFormat.Json;
                req.AddHeader("Prefer", "return=representation");
            }
            else if (method == Method.GET)
            {
                req.AddHeader("Prefer", "odata.include-annotations=*");
            }



            return req;
        }

        public Guid CreateEntity(DVBase entity)
        {

            var entityName = GetEntityName(entity);
            var client = GetRestClient();
            var req = GetRestRequest($"{entityName}s", Method.POST);
            req.AddJsonBody(entity);
            var response = client.Execute(req);

            JObject j = JObject.Parse(response.Content);

            return new Guid(j.Root.SelectToken($"{entityName}id").Value<string>());
        }

        
        public  void CreateEntityImage<T>(Guid entityId, T entity, Expression<Func<T, string>> imageProperty)
        {

            var attrs = entity.GetType().GetCustomAttributes(true);
            var dataContractAttr = attrs.SingleOrDefault(x => x.GetType() == typeof(DataContractAttribute));
            var entityName = (dataContractAttr as DataContractAttribute).Name;

            var expr = (MemberExpression)imageProperty.Body;
            var prop = (PropertyInfo)expr.Member;

            if (string.IsNullOrEmpty((string)prop.GetValue(entity)))
            {
                return;
            }

            var client = GetRestClient();
            var reqSketch = GetRestRequest($"{entityName}s({entityId})/{prop.Name}", Method.PUT);

            reqSketch.AddHeader("Content-Type", "application/octet-stream");
            reqSketch.RequestFormat = DataFormat.None;
            reqSketch.AddParameter("", Convert.FromBase64String((string)prop.GetValue(entity)), ParameterType.RequestBody);
            var respSketch = client.Execute(reqSketch);

        }


        

}
}
