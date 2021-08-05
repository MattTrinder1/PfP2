using API.Models.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace API.DataverseAccess
{
    public sealed class DVDataAccess : DVDataAccessBase
    {
        public ServiceClient DVService { get { return base.dvService; } }

        public DVDataAccess(ServiceClient dvService, IMemoryCache cache)
        {
            base.cache = cache;
            base.dvService = dvService;
        }


        public new Guid GetUserId(string emailAddress)
        {
            return base.GetUserId(emailAddress);
        }

        public T GetEntityByField<T>(string field, object value) where T : DVBase, new()
        {
            return base.GetEntityByField<T>(field, value, DefaultSelectColumns);
        }

        public new T GetEntityByField<T>(string field, object value, SelectColumns selectColumns) where T : DVBase, new()
        {
            return base.GetEntityByField<T>(field, value, selectColumns);
        }


        public ICollection<T> GetAll<T>() where T : DVBase, new()
        {
            return base.GetAll<T>(null, null, null, DefaultSelectColumns);
        }

        public ICollection<T> GetAll<T>(string field, object value) where T : DVBase, new()
        {
            return base.GetAll<T>(field, value, null, DefaultSelectColumns);
        }

        public ICollection<T> GetAll<T>(string field, object value, SelectColumns selectColumns) where T : DVBase, new()
        {
            return base.GetAll<T>(field, value, null, selectColumns);
        }

        public ICollection<T> GetAll<T>(string field, object value, string orderby) where T : DVBase, new()
        {
            return base.GetAll<T>(field, value, orderby, DefaultSelectColumns);
        }

        public new ICollection<T> GetAll<T>(
            string field,
            object value,
            string orderby,
            SelectColumns selectColumns) where T : DVBase, new()
        {
            return base.GetAll<T>(field, value, orderby, selectColumns);
        }


        public new void UpdateEntity<T>(T entity) where T : DVBase
        {
            base.UpdateEntity(entity);
        }

        public new Guid CreateEntity<T>(T entity) where T : DVBase
        {
            return base.CreateEntity(entity);
        }


        public new void CreateEntityImage<T>(
            Guid entityId,
            T entity,
            Expression<Func<T, string>> imageProperty) where T : DVBase
        {
            base.CreateEntityImage<T>(entityId, entity, imageProperty);
        }


        public void GetImages<T>(ICollection<T> collection) where T : DVBase
        {
            base.GetImages(collection, DefaultRequestFullImage, DefaultPrePopulatedImageBehaviour);
        }

        public void GetImages<T>(ICollection<T> collection, bool requestFullImages) where T : DVBase
        {
            base.GetImages(collection, requestFullImages, DefaultPrePopulatedImageBehaviour);
        }

        public new void GetImages<T>(
            ICollection<T> collection,
            bool requestFullImages,
            PrePopulatedImageBehaviour prePopulatedBehaviour) where T : DVBase
        {
            base.GetImages(collection, requestFullImages, prePopulatedBehaviour);
        }

        public void GetImages<T>(T ent) where T : DVBase
        {
            base.GetImages(ent, DefaultRequestFullImage, DefaultPrePopulatedImageBehaviour);
        }

        public void GetImages<T>(T ent, bool requestFullImages) where T : DVBase
        {
            base.GetImages(ent, requestFullImages, DefaultPrePopulatedImageBehaviour);
        }

        public new void GetImages<T>(
            T ent,
            bool requestFullImages,
            PrePopulatedImageBehaviour prePopulatedBehaviour) where T : DVBase
        {
            base.GetImages(ent, requestFullImages, prePopulatedBehaviour);
        }

        public new string GetImage(string entityName, Guid id, string imageColumnName, bool fullImage = true)
        {
            return base.GetImage(entityName, id, imageColumnName, fullImage);
        }
    }
}