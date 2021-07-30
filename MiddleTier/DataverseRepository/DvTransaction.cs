using API.Models.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataverseAccess
{
    public class DVTransaction : DVDataAccess
    {
        public DVTransaction()
        {
            this.transaction = new ExecuteTransactionRequest()
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };
        }

        public void Execute(DVDataAccess executeAs)
        {
            if (executeAs == null) throw new ArgumentNullException("executeAs");
            executeAs.ExecuteTransaction(this.transaction);
        }

        public new void CreateEntity<T>(T entity) where T : DVBase
        {
            base.CreateEntity(entity);
        }
    }
}
