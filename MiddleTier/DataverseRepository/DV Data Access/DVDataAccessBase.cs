using API.Models.Base;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
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
        AllTypeProperties,
        TypePropertiesWithoutImages,
        TypeImagePropertyThumbnails,
        AllTableColumns
    }

    public abstract class DVDataAccessBase
    {
        protected IMemoryCache cache;
        protected ServiceClient dvService;
        protected ExecuteTransactionRequest transaction = null;


        public const SelectColumns DefaultSelectColumns = SelectColumns.TypePropertiesWithoutImages;

        protected DVDataAccessBase() { }

        protected Guid GetUserId(string emailAddress)
        {
            Guid userId;
            if (!cache.TryGetValue($"{emailAddress}:UserId", out userId))
            {
                QueryExpression query = new QueryExpression("systemuser");
                query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, emailAddress);
                query.ColumnSet = new ColumnSet("systemuserid");

                var dvResponse = dvService.RetrieveMultiple(query);

                userId = dvResponse.Entities.Single().GetAttributeValue<Guid>("systemuserid");

                cache.Set($"{emailAddress}:UserId", userId);
            }

            return userId;
        }

        protected string GetEntityName<T>()
        {
            var dataContractAttr = typeof(T).GetCustomAttribute<DataContractAttribute>();
            return (dataContractAttr as DataContractAttribute).Name;
        }
        protected string GetEntityName(DVBase entity)
        {
            var dataContractAttr = entity.GetType().GetCustomAttribute<DataContractAttribute>();
            return (dataContractAttr as DataContractAttribute).Name;
        }


        protected string[] GetPropertyNames<T>(bool includeNonImages = true, bool includeImages = true)
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
            return propertyNames.ToArray();
        }

        protected ColumnSet GetColumnSet<T>(SelectColumns selectColumns = DefaultSelectColumns) where T : DVBase
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


        protected T ConvertFromDvEntity<T>(Entity entity) where T : DVBase, new()
        {
            if (entity == null) return null;

            T result = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.DeclaringType != typeof(T)) continue;

                if (entity.Attributes.Contains(property.Name))
                {
                    Microsoft.Xrm.Sdk.EntityReference sdkEntityReference = entity[property.Name] as Microsoft.Xrm.Sdk.EntityReference;
                    if (sdkEntityReference != null)
                    {
                        Common.Models.Business.EntityReference modelEntityReference = new Common.Models.Business.EntityReference(
                            sdkEntityReference.LogicalName,
                            sdkEntityReference.Id);

                        property.SetValue(result, modelEntityReference);
                    }
                    else
                    {
                        property.SetValue(result, entity[property.Name]);
                    }
                }
            }

            return result;
        }

        protected Entity ConvertToDvEntity<T>(T candidate, bool includeNulls = false) where T : DVBase
        {
            if (candidate == null) return null;

            var entityName = GetEntityName<T>();

            Entity entity = new Entity(entityName);
            if (candidate.Id.HasValue)
            {
                entity.Id = candidate.Id.Value;
            }

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.DeclaringType != typeof(T)) continue;

                var propertyValue = property.GetValue(candidate);
                if (includeNulls || propertyValue != null)
                {
                    Common.Models.Business.EntityReference modelEntityReference = propertyValue as Common.Models.Business.EntityReference;
                    if (modelEntityReference != null)
                    {
                        if (modelEntityReference.EntityId.HasValue)
                        {
                            Microsoft.Xrm.Sdk.EntityReference sdkEntityReference = new Microsoft.Xrm.Sdk.EntityReference(
                                modelEntityReference.EntityLogicalName,
                                modelEntityReference.EntityId.Value);

                            entity.Attributes.Add(property.Name, sdkEntityReference);
                        }
                    }
                    else
                    {
                        DVImageAttribute imageAttribute = property.GetCustomAttribute<DVImageAttribute>();
                        if (imageAttribute == null)
                        {
                            entity.Attributes.Add(property.Name, propertyValue);
                        }
                        else
                        {
                            entity.Attributes.Add(property.Name, Convert.FromBase64String((string)propertyValue));
                        }
                    }
                }
            }

            return entity;
        }


        protected T GetEntityByField<T>(string field, object value, SelectColumns selectColumns = DefaultSelectColumns) where T : DVBase, new()
        {
            QueryExpression query = new QueryExpression(GetEntityName<T>());
            query.Criteria.AddCondition(field, ConditionOperator.Equal, value);
            query.ColumnSet = GetColumnSet<T>(selectColumns);

            var dvResponse = dvService.RetrieveMultiple(query);

            return ConvertFromDvEntity<T>(dvResponse.Entities.SingleOrDefault());
        }

        protected ICollection<T> GetAll<T>(
            string field = null,
            object value = null,
            string orderby = null,
            SelectColumns selectColumns = DefaultSelectColumns) where T : DVBase, new()
        {
            var entityName = GetEntityName<T>();

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

            var dvResponse = dvService.RetrieveMultiple(query);
            if (dvResponse != null && dvResponse.Entities != null && dvResponse.Entities.Count > 0)
            {
                foreach (var dvEntity in dvResponse.Entities)
                {
                    result.Add(ConvertFromDvEntity<T>(dvEntity));
                }
            }

            return result;
        }


        protected void UpdateEntity<T>(T entity) where T : DVBase
        {
            if (entity.Id == null || entity.Id == Guid.Empty) throw new ArgumentException("entity.Id is required.");

            Entity dvEntity = ConvertToDvEntity(entity);
            if (dvEntity != null)
            {
                if (transaction == null)
                {
                    dvService.Update(dvEntity);
                }
                else
                {
                    UpdateRequest updateRequest = new UpdateRequest();
                    updateRequest.Target = dvEntity;
                    transaction.Requests.Add(updateRequest);
                }
            }
        }

        protected Guid CreateEntity<T>(T entity) where T : DVBase
        {
            Guid id = Guid.Empty;

            Entity dvEntity = ConvertToDvEntity(entity);
            if (dvEntity != null)
            {
                if (transaction == null)
                {
                    id = dvService.Create(dvEntity);
                }
                else
                {
                    CreateRequest createRequest = new CreateRequest();
                    createRequest.Target = dvEntity;
                    transaction.Requests.Add(createRequest);
                }
            }

            return id;
        }


        protected void CreateEntityImage<T>(
            Guid entityId,
            T entity,
            Expression<Func<T, string>> imageProperty) where T : DVBase
        {
            string entityName = GetEntityName(entity);

            var expr = (MemberExpression)imageProperty.Body;
            var prop = (PropertyInfo)expr.Member;

            string imageData = (string)prop.GetValue(entity);

            if (string.IsNullOrEmpty(imageData))
            {
                return;
            }

            Entity updateImage = new Entity(entityName, entityId);
            updateImage[prop.Name] = Convert.FromBase64String(imageData);

            if (transaction == null)
            {
                dvService.Update(updateImage);
            }
            else
            {
                UpdateRequest updateRequest = new UpdateRequest();
                updateRequest.Target = updateImage;
                transaction.Requests.Add(updateRequest);
            }
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

        protected void GetImages<T>(
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

        protected void GetImages<T>(
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

        protected string GetImage(string entityName, Guid id, string imageColumnName, bool fullImage = true)
        {
            InitializeFileBlocksDownloadRequest initDlRequest = new InitializeFileBlocksDownloadRequest()
            {
                Target = new EntityReference(entityName, id),
                FileAttributeName = imageColumnName
            };

            var initDlResponse = (InitializeFileBlocksDownloadResponse)dvService.Execute(initDlRequest);

            DownloadBlockRequest dlBlockRequest = new DownloadBlockRequest()
            {
                FileContinuationToken = initDlResponse.FileContinuationToken
            };

            var dlBlockResponse = (DownloadBlockResponse)dvService.Execute(dlBlockRequest);

            //TODO: Handle multiple blocks.

            if (dlBlockResponse != null && dlBlockResponse.Data != null)
            {
                return Convert.ToBase64String(dlBlockResponse.Data);
            }
            return null;
        }
    }
}