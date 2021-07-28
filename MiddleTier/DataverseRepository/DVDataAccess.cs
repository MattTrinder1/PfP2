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

        public enum SelectColumns
        {
            AllTypeProperties,
            TypePropertiesWithoutImages,
            TypeImagePropertyThumbnails,
            AllTableColumns
        }
        public const SelectColumns DefaultSelectColumns = SelectColumns.TypePropertiesWithoutImages;


        public DVDataAccess(ConnectionConfiguration config, IMemoryCache cache, Guid contexUserADObjectId)
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

        private IEnumerable<string> GetPropertyNames<T>(bool includeNonImages = true, bool includeImages = true)
        {
            List<string> propertyNames = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType == typeof(EntityReference)) continue;
                if (property.DeclaringType != typeof(T)) continue;

                if (includeNonImages && includeImages)
                {
                    propertyNames.Add(property.Name);
                }
                else
                {
                    DVImageAttribute imageAttrib = property.GetCustomAttribute<DVImageAttribute>();
                    if ((imageAttrib != null && includeImages) || 
                        (imageAttrib == null && includeNonImages))
                    {
                        propertyNames.Add(property.Name);
                    }
                }
            }
            return propertyNames;
        }

        private string GetSelectColumnNamesCsv<T>(SelectColumns selectColumns = DefaultSelectColumns) where T : DVBase
        {
            IEnumerable<string> selectColumnNames = null;
            switch (selectColumns)
            {
                case SelectColumns.AllTypeProperties:
                    selectColumnNames = GetPropertyNames<T>(true, true);
                    break;

                case SelectColumns.TypePropertiesWithoutImages:
                    selectColumnNames = GetPropertyNames<T>(true, false);
                    break;

                case SelectColumns.TypeImagePropertyThumbnails:
                    selectColumnNames = GetPropertyNames<T>(false, true);
                    break;

                default:
                    break;
            }
            if (selectColumnNames != null)
            {
                return string.Join(',', selectColumnNames);
            }
            return null;
        }

        public T GetEntityByField<T>(string field, string value) where T : DVBase
        {
            return GetEntityByField<T>(field, value, DefaultSelectColumns);
        }

        public T GetEntityByField<T>(string field, string value, SelectColumns selectColumns = DefaultSelectColumns) where T : DVBase
        {
            var client = GetRestClient();

            string selectParam = "";
            string selectColumnNamesCsv = GetSelectColumnNamesCsv<T>(selectColumns);
            if (!string.IsNullOrWhiteSpace(selectColumnNamesCsv))
            {
                selectParam = $"$select={selectColumnNamesCsv}&";
            }
            var req = GetRestRequest($"{SchemaHelpers.GetEntityPluralName(GetEntityName<T>())}?{selectParam}$filter={field} eq '{value}'", Method.GET);
            var resp = client.Execute<DVGETResponse<T>>(req);

            PopulateEntityReferences<T>(resp.Data.value);

            return resp.Data.value.SingleOrDefault();
        }

        public ICollection<T> GetAll<T>() where T : DVBase
        {
            return GetAll<T>(null, null, DefaultSelectColumns);
        }

        public ICollection<T> GetAll<T>(string filter) where T : DVBase
        {
            return GetAll<T>(filter, null, DefaultSelectColumns);
        }

        public ICollection<T> GetAll<T>(string filter, SelectColumns selectColumns) where T : DVBase
        {
            return GetAll<T>(filter, null, selectColumns);
        }

        public ICollection<T> GetAll<T>(string filter, string orderby) where T : DVBase
        {
            return GetAll<T>(filter, orderby, DefaultSelectColumns);
        }

        public ICollection<T> GetAll<T>(string filter, string orderby, SelectColumns selectColumns = DefaultSelectColumns) where T : DVBase
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

            string selectColumnNamesCsv = GetSelectColumnNamesCsv<T>(selectColumns);
            if (!string.IsNullOrWhiteSpace(selectColumnNamesCsv))
            {
                queryParts.Add($"$select={selectColumnNamesCsv}");
            }

            string query = $"{SchemaHelpers.GetEntityPluralName(entityName)}";
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
            if (collection == null) return;

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
                            var matchingPropValue = matchingProp.GetValue(ent);
                            if (matchingPropValue != null)
                            {
                                er = new EntityReference((attr as RelatedEntityNameAttribute).Name, (Guid)matchingPropValue);
                                prop.SetValue(ent, er);
                            }
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
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
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

            if (response.IsSuccessful)
            {
                JObject j = JObject.Parse(response.Content);

                return new Guid(j.Root.SelectToken($"{entityName}id").Value<string>());
            }
            else
            {
                throw new Exception($"CreateEntity request failed: {response.Content}");
            }
        }


        public void CreateEntityImage<T>(Guid entityId, T entity, Expression<Func<T, string>> imageProperty)
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
            var reqSketch = GetRestRequest($"{SchemaHelpers.GetEntityPluralName(entityName)}({entityId})/{prop.Name}", Method.PUT);

            reqSketch.AddHeader("Content-Type", "application/octet-stream");
            reqSketch.RequestFormat = DataFormat.None;
            reqSketch.AddParameter("", Convert.FromBase64String((string)prop.GetValue(entity)), ParameterType.RequestBody);
            var respSketch = client.Execute(reqSketch);

        }

        public enum PrePopulatedImageBehaviour
        {
            Overwrite,
            Retain,
            RegardAsThumbnail,
            RegardAsFullImage
        }
        public const PrePopulatedImageBehaviour DefaultPrePopulatedImageBehaviour = PrePopulatedImageBehaviour.Overwrite;
        public const bool DefaultRequestFullImage = true;

        public void GetImages<T>(ICollection<T> collection) where T : DVBase
        {
            GetImages(collection, DefaultRequestFullImage, DefaultPrePopulatedImageBehaviour);
        }
        public void GetImages<T>(
            ICollection<T> collection,
            bool requestFullImages = true,
            PrePopulatedImageBehaviour prePopulatedBehaviour = PrePopulatedImageBehaviour.Overwrite) where T : DVBase
        {
            if (collection == null) return;

            foreach (var ent in collection)
            {
                GetImages(ent, requestFullImages, prePopulatedBehaviour);
            }
        }

        public void GetImages<T>(T ent) where T : DVBase
        {
            GetImages(ent, DefaultRequestFullImage, DefaultPrePopulatedImageBehaviour);
        }

        public void GetImages<T>(
            T ent, 
            bool requestFullImages = DefaultRequestFullImage, 
            PrePopulatedImageBehaviour prePopulatedBehaviour = DefaultPrePopulatedImageBehaviour) where T : DVBase
        {
            if (ent == null) return;

            string entityName = null;
            var dataContractAttrib = typeof(T).GetCustomAttribute<DataContractAttribute>();
            if (dataContractAttrib != null)
            {
                entityName = dataContractAttrib.Name;
            }

            if (entityName != null)
            {
                foreach (var property in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
                {
                    DVImageAttribute imageAttrib = property.GetCustomAttribute<DVImageAttribute>();
                    if (imageAttrib != null)
                    {
                        var propertyValue = property.GetValue(ent);
                        if (propertyValue is null || 
                            prePopulatedBehaviour == PrePopulatedImageBehaviour.Overwrite || 
                            (prePopulatedBehaviour == PrePopulatedImageBehaviour.RegardAsThumbnail && 
                                imageAttrib.RetrieveImageType == ImageRetrieveBehaviour.AlwaysFullImage) ||
                            (prePopulatedBehaviour == PrePopulatedImageBehaviour.RegardAsFullImage && 
                                imageAttrib.RetrieveImageType == ImageRetrieveBehaviour.AlwaysThumbnail))
                        {
                            bool retrieveFullImage = 
                                (imageAttrib.RetrieveImageType == ImageRetrieveBehaviour.AlwaysFullImage ||
                                (requestFullImages && imageAttrib.RetrieveImageType != ImageRetrieveBehaviour.AlwaysThumbnail));

                            string image = GetImage(entityName, ((DVBase)ent).Id.Value, property.Name, retrieveFullImage);
                            if (image != null)
                            {
                                property.SetValue(ent, image);
                            }
                        }
                    }
                }
            }
        }

        public string GetImage(string entityName, Guid id, string imageColumnName, bool fullImage = true)
        {
            var client = GetRestClient();
            string sizeParam = fullImage ? "?size=full" : "";
            var req = GetRestRequest($"{SchemaHelpers.GetEntityPluralName(entityName)}({id})/{imageColumnName}/$value{sizeParam}", Method.GET);
            req.AddHeader("Content-Type", "application/octet-stream");
            var resp = client.Execute(req);

            if (resp.IsSuccessful)
            {
                return Convert.ToBase64String(resp.RawBytes);
            }
            else
            {
                return null;
            }
        }
    }
}