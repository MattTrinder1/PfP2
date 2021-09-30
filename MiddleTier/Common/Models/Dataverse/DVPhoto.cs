using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_photo")]
    public class DVPhoto : DVEntityBase
    {
        public DVPhoto()
        {
            this.LogicalName = "cp_photo";
        }

        public Guid? cp_photoid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_phototitle { get { return this.GetAttributeValue<string>(nameof(cp_phototitle)); } set { this.Attributes[nameof(cp_phototitle)] = value; } }

        public EntityReference? cp_pocketnotebook { get { return this.GetAttributeValue<EntityReference>(nameof(cp_pocketnotebook)); } set { this.Attributes[nameof(cp_pocketnotebook)] = value; } }
        public EntityReference? cp_suddendeath { get { return this.GetAttributeValue<EntityReference>(nameof(cp_suddendeath)); } set { this.Attributes[nameof(cp_suddendeath)] = value; } }

        public byte[] cp_image { get { return this.GetAttributeValue<byte[]>(nameof(cp_image)); } set { this.Attributes[nameof(cp_image)] = value; } }
    }
}
