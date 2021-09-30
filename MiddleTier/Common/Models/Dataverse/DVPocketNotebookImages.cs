using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_pocketnotebook")]
    public class DVPocketNotebookImages : DVEntityBase
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
