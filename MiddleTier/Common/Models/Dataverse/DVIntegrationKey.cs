using API.Models.Base;
using Common.Models.Business;
using System;
using System.Runtime.Serialization;

namespace API.Models.Dataverse
{
    [DataContract(Name = "cp_integrationkey")]
    public class DVIntegrationKey : DVBase
    {
        public DVIntegrationKey() { }

        public Guid? cp_integrationkeyid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_name { get; set; }

        public DateTime? cp_expiry { get; set; }
    }
}
