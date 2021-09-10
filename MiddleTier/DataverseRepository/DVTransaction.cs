using API.Models.Base;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace API.DataverseAccess
{
    public enum RequestType
    {
        Create,
        CreateImage,
        Update
    }
    public class TransactionRequest
    {

        public TransactionRequest(RequestType requestType, DVBase baseEntity) 
        {
            ReqType = requestType;
            DVBaseEntity = baseEntity;
        }
        public TransactionRequest(RequestType requestType, DVBase baseEntity,Guid entityId, string imagePropertyName)
        {
            ReqType = requestType;
            DVBaseEntity = baseEntity;
            EntityId = entityId;
            ImagePropertyName = imagePropertyName;
        }

        public RequestType ReqType { get; set; }
        public DVBase DVBaseEntity { get; set; }

        public Guid? EntityId { get; set; }

        public string ImagePropertyName { get; set; }

    }

    public sealed class DVTransaction 
    {
        public List<TransactionRequest> Requests;
            

        public DVTransaction()
        {
            Requests = new List<TransactionRequest>();
        }

        public void AddCreateEntity<T>(T entity) where T : DVBase
        {
            Requests.Add(new TransactionRequest(RequestType.Create, entity));
        }

        public void AddCreateEntityImage(
            Guid entityId,
            DVBase entity,
            string imagePropertyName) 
        {
            Requests.Add(new TransactionRequest(RequestType.CreateImage, entity,entityId,imagePropertyName));
        }

        public void AddUpdateEntity<T>(T entity) where T : DVBase
        {
            Requests.Add(new TransactionRequest(RequestType.Update, entity));
        }
    }
}
