using Microsoft.Xrm.Sdk.Client;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_location")]
    public class DVLocation : DVEntityBase
    {
        public DVLocation()
        {
            this.LogicalName = "cp_location";
        }

        public string cp_name { get { return this.GetAttributeValue<string>(nameof(cp_name)); } set { this.Attributes[nameof(cp_name)] = value; } }
        public string cp_housename { get { return this.GetAttributeValue<string>(nameof(cp_housename)); } set { this.Attributes[nameof(cp_housename)] = value; } }
        public string cp_housenumber { get { return this.GetAttributeValue<string>(nameof(cp_housenumber)); } set { this.Attributes[nameof(cp_housenumber)] = value; } }
        public string cp_county { get { return this.GetAttributeValue<string>(nameof(cp_county)); } set { this.Attributes[nameof(cp_county)] = value; } }
        public string cp_description { get { return this.GetAttributeValue<string>(nameof(cp_description)); } set { this.Attributes[nameof(cp_description)] = value; } }
        public string cp_country { get { return this.GetAttributeValue<string>(nameof(cp_country)); } set { this.Attributes[nameof(cp_country)] = value; } }
        public string cp_city { get { return this.GetAttributeValue<string>(nameof(cp_city)); } set { this.Attributes[nameof(cp_city)] = value; } }
        public string cp_street { get { return this.GetAttributeValue<string>(nameof(cp_street)); } set { this.Attributes[nameof(cp_street)] = value; } }
        public string cp_district { get { return this.GetAttributeValue<string>(nameof(cp_district)); } set { this.Attributes[nameof(cp_district)] = value; } }
        public double cp_latitude { get { return this.GetAttributeValue<float>(nameof(cp_latitude)); } set { this.Attributes[nameof(cp_latitude)] = value; } }
        public double cp_longitude { get { return this.GetAttributeValue<float>(nameof(cp_longitude)); } set { this.Attributes[nameof(cp_longitude)] = value; } }


    }
}
