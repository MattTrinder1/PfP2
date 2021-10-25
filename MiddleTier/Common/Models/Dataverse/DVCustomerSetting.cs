using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_customersetting")]
    public class DVCustomerSetting : DVEntityBase
    {
        public DVCustomerSetting()
        {
            this.LogicalName = "cp_customersetting";
        }

        public Guid? cp_customersettingid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_customername { get { return this.GetAttributeValue<string>(nameof(cp_customername)); } set { this.Attributes[nameof(cp_customername)] = value; } }
        public bool cp_active { get { return this.GetAttributeValue<bool>(nameof(cp_active)); } set { this.Attributes[nameof(cp_active)] = value; } }
        public string cp_incidentprefix { get { return this.GetAttributeValue<string>(nameof(cp_incidentprefix)); } set { this.Attributes[nameof(cp_incidentprefix)] = value; } }
        public string cp_policingareatext { get { return this.GetAttributeValue<string>(nameof(cp_policingareatext)); } set { this.Attributes[nameof(cp_policingareatext)] = value; } }
    }
}
