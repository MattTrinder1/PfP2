using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_integrationkey")]
    public class DVIntegrationKey : DVEntityBase
    {
        public DVIntegrationKey() 
        {
            this.LogicalName = "cp_integrationkey"; 
        }

        public Guid? cp_integrationkeyid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_name { get; set; }

        public DateTime? cp_expiry { get; set; }

        public string cp_restrictedto { get; set; }
    }
}
