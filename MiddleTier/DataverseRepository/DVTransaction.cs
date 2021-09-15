using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

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

        public TransactionRequest(RequestType requestType, Entity target) 
        {
            ReqType = requestType;
            Target = target;
        }
        public TransactionRequest(RequestType requestType, Entity target, string imagePropertyName)
        {
            ReqType = requestType;
            Target = target;
            ImagePropertyName = imagePropertyName;
        }

        public RequestType ReqType { get; set; }
        public Entity Target { get; set; }

        public string ImagePropertyName { get; set; }

    }

    public sealed class DVTransaction 
    {
        public List<TransactionRequest> Requests;
            

        public DVTransaction()
        {
            Requests = new List<TransactionRequest>();
        }

        public void AddCreateEntity<T>(T entity) where T : Entity
        {
            Requests.Add(new TransactionRequest(RequestType.Create, entity));
        }

        public void AddCreateEntityImage(
            Entity entity,
            string imagePropertyName) 
        {
            Requests.Add(new TransactionRequest(RequestType.CreateImage, entity,imagePropertyName));
        }

        public void AddUpdateEntity<T>(T entity) where T : Entity
        {
            Requests.Add(new TransactionRequest(RequestType.Update, entity));
        }
    }
}
