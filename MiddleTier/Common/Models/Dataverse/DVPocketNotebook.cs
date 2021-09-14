using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace API.Models.PNB
{
    [EntityLogicalName("cp_pocketnotebook")]
    public class DVPocketNotebook : Entity
    {
        public DVPocketNotebook()
        {
            this.LogicalName = "cp_pocketnotebook";
        }

        public Guid? cp_pocketnotebookid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_notes { get { return this.GetAttributeValue<string>(nameof(cp_notes)); } set { this.Attributes[nameof(cp_notes)] = value; } }

        public DateTime? cp_notedateandtime { get { return this.GetAttributeValue<DateTime?>(nameof(cp_notedateandtime)); } set { this.Attributes[nameof(cp_notedateandtime)] = value; } }

        public EntityReference cp_incidentno { get { return this.GetAttributeValue<EntityReference>(nameof(cp_incidentno)); } set { this.Attributes[nameof(cp_incidentno)] = value; } }

        public EntityReference cp_enteredby { get { return this.GetAttributeValue<EntityReference>(nameof(cp_enteredby)); } set { this.Attributes[nameof(cp_enteredby)] = value; } }

        public DateTime? cp_signaturedateandtime { get { return this.GetAttributeValue<DateTime?>(nameof(cp_signaturedateandtime)); } set { this.Attributes[nameof(cp_signaturedateandtime)] = value; } }
        public string cp_signatoryname { get { return this.GetAttributeValue<string>(nameof(cp_signatoryname)); } set { this.Attributes[nameof(cp_signatoryname)] = value; } }
    }


    [EntityLogicalName("cp_pocketnotebook")]
    public class DVPocketNotebookImages : Entity
    {
        public DVPocketNotebookImages()
        {
            this.LogicalName = "cp_pocketnotebook";
        }

        public Guid? cp_pocketnotebookid { get { return base.Id; } set { base.Id = value.Value; } }

        public byte[] cp_sketch { get { return this.GetAttributeValue<byte[]>(nameof(cp_sketch)); } set { this.Attributes[nameof(cp_sketch)] = value; } }

        public byte[] cp_signature { get { return this.GetAttributeValue<byte[]>(nameof(cp_signature)); } set { this.Attributes[nameof(cp_signature)] = value; } }
    }
}
