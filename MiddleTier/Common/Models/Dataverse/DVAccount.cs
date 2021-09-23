using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace API.Models.Dataverse
{
    [EntityLogicalName("account")]
    public class DVAccount : DVEntityBase
    {
        public DVAccount()
        {
            this.LogicalName = "account";
        }

        public string name   { get { return this.GetAttributeValue<string>(nameof(name)); } set { this.Attributes[nameof(name)] = value; } }
        public string address1_city { get { return this.GetAttributeValue<string>(nameof(address1_city)); } set { this.Attributes[nameof(address1_city)] = value; } }
        public string address1_line1 { get { return this.GetAttributeValue<string>(nameof(address1_line1)); } set { this.Attributes[nameof(address1_line1)] = value; } }
        public string address1_line2 { get { return this.GetAttributeValue<string>(nameof(address1_line2)); } set { this.Attributes[nameof(address1_line2)] = value; } }
        public string address1_line3 { get { return this.GetAttributeValue<string>(nameof(address1_line3)); } set { this.Attributes[nameof(address1_line3)] = value; } }
        public string address1_postalcode { get { return this.GetAttributeValue<string>(nameof(address1_postalcode)); } set { this.Attributes[nameof(address1_postalcode)] = value; } }
        public string address1_county { get { return this.GetAttributeValue<string>(nameof(address1_county)); } set { this.Attributes[nameof(address1_county)] = value; } }
        public string address1_country { get { return this.GetAttributeValue<string>(nameof(address1_country)); } set { this.Attributes[nameof(address1_country)] = value; } }
        public string telephone1 { get { return this.GetAttributeValue<string>(nameof(telephone1)); } set { this.Attributes[nameof(telephone1)] = value; } }
        public bool cp_isgp { get { return this.GetAttributeValue<bool>(nameof(cp_isgp)); } set { this.Attributes[nameof(cp_isgp)] = value; } }
        public string cp_surgery { get { return this.GetAttributeValue<string>(nameof(cp_surgery)); } set { this.Attributes[nameof(cp_surgery)] = value; } }

    }
}
