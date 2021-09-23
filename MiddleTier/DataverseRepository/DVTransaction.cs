using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace API.DataverseAccess
{
    public enum RequestType
    {
        Create,
        CreateImage,
        Update,
        Associate
    }
    public class TransactionRequest
    {

        public TransactionRequest(RequestType requestType, Entity target)
        {
            ReqType = requestType;
            Target = target;
        }
        public TransactionRequest(RequestType requestType, EntityReference id1, EntityReference id2,string relationshipName)
        {
            ReqType = requestType;
            Id1 = id1;
            Id2 = id2;
            RelationshipName = relationshipName;
        }
        public TransactionRequest(RequestType requestType, Entity target, string imagePropertyName)
        {
            ReqType = requestType;
            Target = target;
            ImagePropertyName = imagePropertyName;
        }

        public RequestType ReqType { get; set; }
        public Entity Target { get; set; }

        public EntityReference Id1 { get; set; }
        public EntityReference Id2 { get; set; }
        public string RelationshipName { get; set; }

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
        public void AddAssociateEntities(EntityReference id1,EntityReference id2,string relationshipName) 
        {
            Requests.Add(new TransactionRequest(RequestType.Associate, id1,id2,relationshipName));
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
