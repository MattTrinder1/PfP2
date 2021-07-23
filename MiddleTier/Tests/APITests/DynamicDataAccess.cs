using API.Models.Base;
using API.Models.IYC;
using Common.Models.Business;
using Common.Models.Dataverse;
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
    public class DynamicDataAccess
    {
        private string _dvUrl;
        private string _serviceUrl;
        private string _clientId;
        private string _clientSecret;
        private string _authority;
        public DynamicDataAccess(string dvUrl, string serviceUrl, string clientId,string clientSecret,string authority)
        {
            _dvUrl = dvUrl;
            _serviceUrl = serviceUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _authority = authority;
        }

        public async Task<string> GetAuthToken()
        {

            var authContext = new AuthenticationContext(_authority);
            ClientCredential credential = new ClientCredential(_clientId, _clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(_serviceUrl, credential);
            return result.AccessToken;

        }

        private string GetEntityName<T>()
        {
            var attrs = typeof(T).GetCustomAttributes(true);
            var dataContractAttr = attrs.SingleOrDefault(x => x.GetType() == typeof(DataContractAttribute));
            return (dataContractAttr as DataContractAttribute).Name;
        }

        
        public dynamic GetEntityByField(string entityName, string fieldName,string fieldValue)
        {
            var client = GetRestClient();
            var req = GetRestRequest($"{SchemaHelpers.GetEntityPluralName(entityName)}?$filter={fieldName} eq '{fieldValue}'", Method.GET);
            var resp = client.Execute(req);

            var a = JsonSerializer.Deserialize<JsonElement>(resp.Content);
            return GetDynamicEntity(a.GetProperty("value")[0]);

        }

        private static DynamicEntity GetDynamicEntity(JsonElement a)
        {
            var ret = new DynamicEntity();
            foreach (var prop in a.EnumerateObject())
            {
                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        if (DateTime.TryParse(prop.Value.GetString(), out DateTime date))
                        {
                            ret.AddProperty(prop.Name, date);
                        }
                        else if (Guid.TryParse(prop.Value.GetString(), out Guid guid))
                        {
                            ret.AddProperty(prop.Name, guid);
                        }
                        else
                        {
                            ret.AddProperty(prop.Name, prop.Value.GetString());
                        }

                        break;
                    case JsonValueKind.False:
                    case JsonValueKind.True:
                        ret.AddProperty(prop.Name, prop.Value.GetBoolean());
                        break;
                    case JsonValueKind.Number:
                        if (prop.Value.ToString().Contains("."))
                        {
                            ret.AddProperty(prop.Name, prop.Value.GetDecimal());
                        }
                        else if (prop.Value.TryGetInt32( out int num))
                        { ret.AddProperty(prop.Name, prop.Value.GetInt32()); }
                        else
                        { ret.AddProperty(prop.Name, prop.Value.GetInt64()); }

                        
                        break;
                    case JsonValueKind.Null:
                        ret.AddProperty(prop.Name, null);
                        break;
                    default:
                        ret.AddProperty(prop.Name, prop.Value.GetString());
                        break;
                }


            }

            return ret;
        }

        public dynamic GetEntity(string entityName, Guid entityId)
        {
            var client = GetRestClient();
            var req = GetRestRequest($"{SchemaHelpers.GetEntityPluralName(entityName)}({entityId})", Method.GET);
            var resp = client.Execute(req);

            //var entity = resp.Data;

            //PopulateEntityReferences(entity);

            var a = JsonSerializer.Deserialize<JsonElement>(resp.Content);
            return GetDynamicEntity(a);

        }

        public List<dynamic> GetAll(string entityName, string filter)
        {
            var client = GetRestClient();
            var req = GetRestRequest($"{SchemaHelpers.GetEntityPluralName(entityName)}?$filter={filter}", Method.GET);
            var resp = client.Execute(req);

            //var entity = resp.Data;

            //PopulateEntityReferences(entity);

            var a = JsonSerializer.Deserialize<JsonElement>(resp.Content);
            var list = new List<dynamic>();
            foreach (var value in a.GetProperty("value").EnumerateArray())
            {
                list.Add(GetDynamicEntity(value));
            }
            return list;

        }

        //public ExpandoObject GetEntity(string entityName,Guid entityId)
        //{
        //    //var attrs = typeof(T).GetCustomAttributes(true);
        //    //var dataContractAttr = attrs.SingleOrDefault(x => x.GetType() == typeof(DataContractAttribute));
        //    //var entityName = (dataContractAttr as DataContractAttribute).Name;

        //    var client = GetRestClient();
        //    var req = GetRestRequest($"{GetEntityPluralName(entityName)}({entityId})", Method.GET);
        //    var resp = client.Execute<ExpandoObject>(req);

        //    var entity = resp.Data;

        //    //PopulateEntityReferences(entity);

        //    return entity;

        //}



        private RestClient GetRestClient()
        {
            RestClient client;

            client = new RestClient(_dvUrl);
            client.AddDefaultHeader("Authorization", "Bearer " + GetAuthToken().Result);
            //client.AddHandler("application/json", () => { return new IntegrationService.JsonDeserializer(); });

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
                ,
               // PropertyNamingPolicy = new LookupNamingPolicy(),
                
            };
            options.Converters.Add(new LookupJsonConverter());

           client.UseSystemTextJson(options);

           // client.UseSerializer<CustomJsonSerializer>();

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

        

        public  Guid GetUserId(string emailAddress)
        {
            var client = GetRestClient();
            var req = GetRestRequest($"systemusers?$filter=internalemailaddress eq '{emailAddress}'&$select=systemuserid", Method.GET);
            var response = client.Execute<DVGETResponse<DVSystemUser>>(req);

            return response.Data.value.Single().SystemUserId;

        }



        

}
}
