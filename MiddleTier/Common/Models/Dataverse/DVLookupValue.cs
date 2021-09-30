using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_lookupvalue")]
    public class DVLookupValue : DVEntityBase
    {
        public DVLookupValue()
        {
            this.LogicalName = "cp_lookupvalue";
        }

        public Guid? cp_lookupvalueid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_name { get { return this.GetAttributeValue<string>(nameof(cp_name)); } set { this.Attributes[nameof(cp_name)] = value; } }

        public string cp_alternativedisplayname { get { return this.GetAttributeValue<string>(nameof(cp_alternativedisplayname)); } set { this.Attributes[nameof(cp_alternativedisplayname)] = value; } }

        public int? cp_displaysequenceno { get { return this.GetAttributeValue<int>(nameof(cp_displaysequenceno)); } set { this.Attributes[nameof(cp_displaysequenceno)] = value; } }
    }
}
