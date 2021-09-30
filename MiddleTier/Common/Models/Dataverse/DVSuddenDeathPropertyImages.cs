using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_suddendeathproperty")]
    public class DVSuddenDeathPropertyImages : DVEntityBase
    {
        public DVSuddenDeathPropertyImages()
        {
            this.LogicalName = "cp_suddendeathproperty";
        }

        public Guid? cp_suddendeathpropertyid { get { return base.Id; } set { base.Id = value.Value; } }

        public byte[] cp_propertyphoto { get { return this.GetAttributeValue<byte[]>(nameof(cp_propertyphoto)); } set { this.Attributes[nameof(cp_propertyphoto)] = value; } }
        public byte[] cp_signature { get { return this.GetAttributeValue<byte[]>(nameof(cp_signature)); } set { this.Attributes[nameof(cp_signature)] = value; } }

        
    }
}
