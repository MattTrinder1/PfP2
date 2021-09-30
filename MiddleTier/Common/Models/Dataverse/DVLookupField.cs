using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_lookupfield")]
    public class DVLookupField : DVEntityBase
    {
        public DVLookupField()
        {
            this.LogicalName = "cp_lookupfield";
        }

        public Guid? cp_lookupfieldid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_displayname { get { return this.GetAttributeValue<string>(nameof(cp_displayname)); } set { this.Attributes[nameof(cp_displayname)] = value; } }

        public string cp_id { get { return this.GetAttributeValue<string>(nameof(cp_id)); } set { this.Attributes[nameof(cp_id)] = value; } }

        public int? cp_totalvalues { get { return this.GetAttributeValue<int>(nameof(cp_totalvalues)); } set { this.Attributes[nameof(cp_totalvalues)] = value; } }

        public int? cp_usedin { get { return this.GetAttributeValue<int>(nameof(cp_usedin)); } set { this.Attributes[nameof(cp_usedin)] = value; } }
    }
}
