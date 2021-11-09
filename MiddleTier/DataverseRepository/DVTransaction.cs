using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.DataverseAccess
{
    public enum RequestType
    {
        Create,
        CreateImage,
        Update,
        Associate,
        Disassociate,
        Upsert,
        Delete,
        SetState
    }

    

    public class TransactionRequest
    {

        public TransactionRequest(RequestType requestType, Entity target)
        {
            ReqType = requestType;
            Target = target;
        }
        public TransactionRequest(RequestType requestType, EntityReference target)
        {
            ReqType = requestType;
            TargetReference = target;
        }
        public TransactionRequest(RequestType requestType, EntityReference target,int state,int status)
        {
            ReqType = requestType;
            TargetReference = target;
            State = state;
            Status = status;
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
        public EntityReference TargetReference { get; set; }

        
        public EntityReference Id1 { get; set; }
        public EntityReference Id2 { get; set; }
        public string RelationshipName { get; set; }

        public string ImagePropertyName { get; set; }
        public int State { get; set; }
        public int Status { get; set; }

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
        public void AddUpsertEntity<T>(T entity) where T : Entity
        {
            Requests.Add(new TransactionRequest(RequestType.Upsert, entity));
        }

        public void AddSetStateRequest(EntityReference target, int state, int status)
        {
            Requests.Add(new TransactionRequest(RequestType.SetState, target,state,status));

        }
        public void AddDeleteRequest(EntityReference target)
        {
            //check that we don't already have a delete request for this target
            if (!Requests.Any(x => x.ReqType == RequestType.Delete && x.TargetReference.LogicalName == target.LogicalName && x.TargetReference.Id == target.Id))
            {
                Requests.Add(new TransactionRequest(RequestType.Delete, target));
            }
        }
        public void AddAssociateEntities(EntityReference id1, EntityReference id2, string relationshipName)
        {
            Requests.Add(new TransactionRequest(RequestType.Associate, id1, id2, relationshipName));
        }
        public void AddDisassociateEntities(EntityReference id1, EntityReference id2, string relationshipName)
        {
            Requests.Add(new TransactionRequest(RequestType.Disassociate, id1, id2, relationshipName));
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
