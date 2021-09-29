using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using MoD.CAMS.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace API.DataverseAccess
{
    public enum SelectColumns
    {
        /// <summary>
        /// Retrieve columns represented by the properties of the target DV class.
        /// </summary>
        AllTypeProperties,
        /// <summary>
        /// Retrieve columns represented by the properties of the target DV class excluding images.
        /// </summary>
        TypePropertiesWithoutImages,
        /// <summary>
        /// Retrieve thumbnails for image columns represented by the (byte[]) properties of the target DV class.
        /// </summary>
        TypeImagePropertyThumbnails,
        /// <summary>
        /// Retrieve all columns of the DataVerse table.
        /// </summary>
        AllTableColumns
    }

    public class DVDataAccess
    {
        private IMemoryCache _cache;
        private IOrganizationService _dvService;
        private Guid? _userId;

        public const SelectColumns DefaultSelectColumns = SelectColumns.TypePropertiesWithoutImages;


        public DVDataAccess(IOrganizationService dvService, IMemoryCache cache)
        {
            _cache = cache;
            _dvService = dvService;
        }

        public DVDataAccess(IOrganizationService dvService, IMemoryCache cache, Guid userId)
        {
            _cache = cache;
            _dvService = dvService;
            _userId = userId;
        }

        public Guid? UserId { get { return _userId; } }

        public ExecuteTransactionResponse ExecuteTransaction(DVTransaction trans)
        {
            var transaction = new ExecuteTransactionRequest()
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };


            foreach (var transReq in trans.Requests)
            {

                switch (transReq.ReqType)
                {
                    case RequestType.Create:
                        var createReq = new CreateRequest();
                        createReq.Target = transReq.Target;
                        transaction.Requests.Add(createReq);
                        break;
                    case RequestType.CreateImage:

                        string entityName = transReq.Target.LogicalName;


                        var prop = transReq.Target.GetType().GetProperty(transReq.ImagePropertyName);
                        var imageData = (byte[])prop.GetValue(transReq.Target);

                        if (imageData.Any())
                        {
                            Entity updateImage = new Entity(entityName, transReq.Target.Id);
                            updateImage[prop.Name] = imageData;

                            var updateImageReq = new UpdateRequest();
                            updateImageReq.Target = updateImage;
                            transaction.Requests.Add(updateImageReq);
                        }


                        break;
                    case RequestType.Update:
                        var updateReq = new UpdateRequest();
                        updateReq.Target = transReq.Target;
                        transaction.Requests.Add(updateReq);

                        break;
                    case RequestType.Associate:
                        var associateReq = new AssociateEntitiesRequest();
                        associateReq.Moniker1 = transReq.Id1;
                        associateReq.Moniker2 = transReq.Id2;
                        associateReq.RelationshipName = transReq.RelationshipName;
                        transaction.Requests.Add(associateReq);

                        break;
                }



            }
            return (ExecuteTransactionResponse)_dvService.Execute(transaction);
        }

        public OptionSetValue GetOptionSetValue(string optionSetName, string optionSetText)
        {

            return GetOptionSetValue(optionSetName, optionSetText, "");
        }
        public OptionSetValue GetOptionSetValue(string optionSetName, string optionSetText, string entityName = "")
        {

            if (optionSetText == null)
            {
                return null;
            }

            var value = DynamicsServiceHelper.GetOptionSetValue(_dvService, optionSetName, optionSetText, entityName);
            if (value == 0)
            {
                return null;
            }
            return new OptionSetValue(value);
        }

        public Guid GetUserId(string emailAddress)
        {
            Guid userId;
            if (!_cache.TryGetValue($"{emailAddress}:UserId", out userId))
            {
                QueryExpression query = new QueryExpression("systemuser");
                query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, emailAddress);
                query.ColumnSet = new ColumnSet("systemuserid");

                var dvResponse = _dvService.RetrieveMultiple(query);

                userId = dvResponse.Entities.Single().GetAttributeValue<Guid>("systemuserid");

                _cache.Set($"{emailAddress}:UserId", userId);
            }

            return userId;
        }

      
       



        public string[] GetPropertyNames<T>(bool includeNonImages = true, bool includeImages = true)
        {
            List<string> propertyNames = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                //if (property.PropertyType == typeof(EntityReference)) continue;
                if (property.DeclaringType != typeof(T)) continue;

                if (includeNonImages && includeImages)
                {
                    propertyNames.Add(property.Name);
                }
                else
                {
                    bool isImage = (property.PropertyType == typeof(byte[]));
                    if (isImage)
                    {
                        DVNotImageAttribute notImageAttrib = property.GetCustomAttribute<DVNotImageAttribute>();
                        isImage = (notImageAttrib == null);
                    }
                    if ((isImage && includeImages) || 
                        (!isImage && includeNonImages))
                    {
                        propertyNames.Add(property.Name);
                    }
                }
            }
            return propertyNames.ToArray();
        }

        public ColumnSet GetColumnSet<T>(SelectColumns selectColumns = DefaultSelectColumns) where T : Entity
        {
            string[] selectColumnNames = null;
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

                case SelectColumns.AllTableColumns:
                    return new ColumnSet(true);

                default:
                    break;
            }
            if (selectColumnNames != null)
            {
                return new ColumnSet(selectColumnNames);
            }
            return null;
        }


        //public T ConvertFromDvEntity<T>(Entity entity) where T : DVBase, new()
        //{
        //    if (entity == null) return null;

        //    T result = new T();

        //    foreach (var property in typeof(T).GetProperties())
        //    {
        //        if (property.DeclaringType != typeof(T)) continue;

        //        if (entity.Attributes.Contains(property.Name))
        //        {
        //            Microsoft.Xrm.Sdk.EntityReference sdkEntityReference = entity[property.Name] as Microsoft.Xrm.Sdk.EntityReference;
        //            if (sdkEntityReference != null)
        //            {
        //                Common.Models.Business.EntityRef modelEntityReference = new Common.Models.Business.EntityRef(
        //                    sdkEntityReference.LogicalName,
        //                    sdkEntityReference.Id);

        //                property.SetValue(result, modelEntityReference);
        //            }
        //            else
        //            {
        //                Microsoft.Xrm.Sdk.OptionSetValue optionSetValue = entity[property.Name] as Microsoft.Xrm.Sdk.OptionSetValue;
        //                if (optionSetValue != null)
        //                {
        //                    property.SetValue(result, optionSetValue.Value);
        //                }
        //                else
        //                {
        //                    property.SetValue(result, entity[property.Name]);
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}

        //public Entity ConvertToDvEntity<T>(T candidate, bool includeNulls = false) where T : DVBase
        //{
        //    if (candidate == null) return null;

        //    var entityName = GetEntityName(candidate);

        //    Entity entity = new Entity(entityName);
        //    if (candidate.Id.HasValue)
        //    {
        //        entity.Id = candidate.Id.Value;
        //    }

        //    foreach (var property in candidate.GetType().GetProperties())
        //    {
        //        if (property.DeclaringType != candidate.GetType()) continue;

        //        var propertyValue = property.GetValue(candidate);
        //        if (includeNulls || propertyValue != null)
        //        {
        //            Common.Models.Business.EntityRef modelEntityReference = propertyValue as Common.Models.Business.EntityRef;
        //            if (modelEntityReference != null)
        //            {
        //                if (modelEntityReference.EntityId.HasValue)
        //                {
        //                    Microsoft.Xrm.Sdk.EntityReference sdkEntityReference = new Microsoft.Xrm.Sdk.EntityReference(
        //                        modelEntityReference.EntityLogicalName,
        //                        modelEntityReference.EntityId.Value);

        //                    entity.Attributes.Add(property.Name, sdkEntityReference);
        //                }
        //            }
        //            else
        //            {
        //                DVImageAttribute imageAttribute = property.GetCustomAttribute<DVImageAttribute>();
        //                if (imageAttribute == null)
        //                {
        //                    entity.Attributes.Add(property.Name, propertyValue);
        //                }
        //                else
        //                {
        //                    entity.Attributes.Add(property.Name, Convert.FromBase64String((string)propertyValue));
        //                }
        //            }
        //        }
        //        if (property.Name.ToLower() == "cp_enteredby" && _userId.HasValue)
        //        {
        //            entity.Attributes.Add("cp_enteredby",new EntityReference("systemuser",_userId.Value));

        //        }
        //    }

        //    return entity;
        //}

        public List<Entity> GetMultiple(QueryExpression q)
        {
            return DynamicsServiceHelper.GetMultiple(_dvService, q);
        }


        public T GetEntityByField<T>(string field, object value, SelectColumns selectColumns = DefaultSelectColumns) where T : Entity, new()
        {
            QueryExpression query = new QueryExpression(new T().LogicalName);
            query.Criteria.AddCondition(field, ConditionOperator.Equal, value);
            query.ColumnSet = GetColumnSet<T>(selectColumns);

            var ent = _dvService.RetrieveMultiple(query).Entities;
            if (ent.Any())
            {
                return ent.Single().ToEntity<T>();
            }
            else
            {
                return null;
            }


            //return ConvertFromDvEntity<T>(dvResponse.Entities.SingleOrDefault());
        }

        public object GetEntityFieldValue(string entityName,Guid entityId, string fieldName)
        {
            var entity = _dvService.GetEntity(entityName, entityId);
            return entity[fieldName];
            
        }


        public Guid? GetEntityId(string entityName, string fieldName, object value)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.Criteria.AddCondition(fieldName, ConditionOperator.Equal, value);

            var dvResponse = _dvService.RetrieveMultiple(query);
            if (dvResponse.Entities.SingleOrDefault() != null)
            {
                return dvResponse.Entities.Single().Id;
            }
            else
            {
                return null;
            }


        }

        public T GetEntityByFields<T>(IEnumerable<KeyValuePair<string,object>> fieldValues, SelectColumns selectColumns = DefaultSelectColumns) where T : Entity, new()
        {
            QueryExpression query = new QueryExpression(new T().LogicalName);
            foreach (var keyValuePair in fieldValues)
            {
                query.Criteria.AddCondition(keyValuePair.Key, ConditionOperator.Equal, keyValuePair.Value);
            }
            query.ColumnSet = GetColumnSet<T>(selectColumns);

            return _dvService.RetrieveMultiple(query).Entities.SingleOrDefault().ToEntity<T>() ;

            
        }

        public ICollection<T> GetAll<T>(string field, object value, SelectColumns selectColumns) where T : Entity, new()
        {
            return GetAll<T>(field, value, null, selectColumns);
        }


        public List<T> GetAll<T>(
            string field = null,
            object value = null,
            string orderby = null,
            SelectColumns selectColumns = DefaultSelectColumns) where T : Entity, new()
        {
            var entityName = new T().LogicalName;

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = GetColumnSet<T>(selectColumns);

            if (!string.IsNullOrEmpty(field))
            {
                query.Criteria.AddCondition(field, ConditionOperator.Equal, value);
            }

            if (!string.IsNullOrEmpty(orderby))
            {
                query.AddOrder(orderby, OrderType.Ascending);
            }

            List<T> result = new List<T>();

            foreach (var e in _dvService.RetrieveMultiple(query).Entities)
            {
                result.Add(e.ToEntity<T>());
            }

            return result;

            
            
        }

        public static string GetEntityLogicalName<T>(T ent) where T: Entity
        {
            if (!string.IsNullOrEmpty(ent.LogicalName)) return ent.LogicalName;

            var logicalNameAttrib = typeof(T).GetCustomAttribute<EntityLogicalNameAttribute>();
            if (logicalNameAttrib != null) return logicalNameAttrib.LogicalName;

            return null;
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

        public void GetImages<T>(
            ICollection<T> collection,
            bool requestFullImages = true,
            PrePopulatedImageBehaviour prePopulatedBehaviour = PrePopulatedImageBehaviour.Overwrite) where T : Entity
        {
            if (collection == null) return;

            foreach (var ent in collection)
            {
                GetImages(ent, requestFullImages, prePopulatedBehaviour);
            }
        }

        public void GetImages<T>(
            T ent,
            bool requestFullImages = DefaultRequestFullImage,
            PrePopulatedImageBehaviour prePopulatedBehaviour = DefaultPrePopulatedImageBehaviour) where T : Entity
        {
            if (ent == null) return;

            string entityName = GetEntityLogicalName(ent);

            if (entityName != null)
            {
                foreach (var property in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(byte[])))
                {
                    DVNotImageAttribute notImageAttrib = property.GetCustomAttribute<DVNotImageAttribute>();
                    if (notImageAttrib != null) continue;

                    DVImageAttribute imageAttrib = property.GetCustomAttribute<DVImageAttribute>();

                    var propertyValue = property.GetValue(ent);
                    if (propertyValue is null || 
                        prePopulatedBehaviour == PrePopulatedImageBehaviour.Overwrite || 
                        imageAttrib == null || 
                        (prePopulatedBehaviour == PrePopulatedImageBehaviour.RegardAsThumbnail && 
                            imageAttrib.RetrieveImageType == ImageRetrieveBehaviour.AlwaysFullImage) ||
                        (prePopulatedBehaviour == PrePopulatedImageBehaviour.RegardAsFullImage &&
                            imageAttrib.RetrieveImageType == ImageRetrieveBehaviour.AlwaysThumbnail))
                    {
                        bool retrieveFullImage =
                            (requestFullImages && (imageAttrib == null || imageAttrib.RetrieveImageType != ImageRetrieveBehaviour.AlwaysThumbnail)) || 
                            (imageAttrib != null && imageAttrib.RetrieveImageType == ImageRetrieveBehaviour.AlwaysFullImage);

                        var image = GetImage(entityName, ent.Id, property.Name, retrieveFullImage);
                        if (image != null)
                        {
                            property.SetValue(ent, image);
                        }
                    }
                }
            }
        }

        protected byte[] GetImage(string entityName, Guid id, string imageColumnName, bool fullImage = true)
        {
            InitializeFileBlocksDownloadRequest initDlRequest = new InitializeFileBlocksDownloadRequest()
            {
                Target = new EntityReference(entityName, id),
                FileAttributeName = imageColumnName
            };

            var initDlResponse = (InitializeFileBlocksDownloadResponse)_dvService.Execute(initDlRequest);

            DownloadBlockRequest dlBlockRequest = new DownloadBlockRequest()
            {
                FileContinuationToken = initDlResponse.FileContinuationToken
            };

            var dlBlockResponse = (DownloadBlockResponse)_dvService.Execute(dlBlockRequest);

            //TODO: Handle multiple blocks.

            if (dlBlockResponse != null)
            {
                return dlBlockResponse.Data;
            }
            return null;
        }
    }
}