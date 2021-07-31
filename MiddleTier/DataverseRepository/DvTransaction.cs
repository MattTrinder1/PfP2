using API.Models.Base;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq.Expressions;

namespace API.DataverseAccess
{
    public sealed class DVTransaction : DVDataAccessBase
    {
        public DVTransaction()
        {
            base.transaction = new ExecuteTransactionRequest()
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };
        }

        public ExecuteTransactionResponse Execute(ServiceClient service)
        {
            if (service == null) throw new ArgumentNullException("service");
            return (ExecuteTransactionResponse)service.Execute(base.transaction);
        }

        public void AddCreateEntity<T>(T entity) where T : DVBase
        {
            base.CreateEntity(entity);
        }

        public void AddCreateEntityImage<T>(
            Guid entityId,
            T entity,
            Expression<Func<T, string>> imageProperty) where T : DVBase
        {
            base.CreateEntityImage<T>(entityId, entity, imageProperty);
        }
    }
}
