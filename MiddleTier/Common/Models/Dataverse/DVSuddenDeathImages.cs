using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace API.Models.PNB
{
    [EntityLogicalName("cp_suddendeath")]
    public class DVSuddenDeathImages : DVEntityBase
    {
        public DVSuddenDeathImages()
        {
            this.LogicalName = "cp_suddendeath";
        }

        public Guid? cp_suddendeathid { get { return base.Id; } set { base.Id = value.Value; } }

        public byte[] cp_suicidenotepicture { get { return this.GetAttributeValue<byte[]>(nameof(cp_suicidenotepicture)); } set { this.Attributes[nameof(cp_suicidenotepicture)] = value; } }
        public byte[] cp_identificationsignature { get { return this.GetAttributeValue<byte[]>(nameof(cp_identificationsignature)); } set { this.Attributes[nameof(cp_identificationsignature)] = value; } }

        
        //public byte[] cp_signature { get { return this.GetAttributeValue<byte[]>(nameof(cp_signature)); } set { this.Attributes[nameof(cp_signature)] = value; } }
    }
}
