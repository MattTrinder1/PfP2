using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace API.Models.Dataverse
{
    [EntityLogicalName("contact")]
    public class DVContact : DVEntityBase
    {
        public DVContact()
        {
            this.LogicalName = "contact";
        }

        
        public string firstname { get { return this.GetAttributeValue<string>(nameof(firstname)); } set { this.Attributes[nameof(firstname)] = value; } }
        public string lastname { get { return this.GetAttributeValue<string>(nameof(lastname)); } set { this.Attributes[nameof(lastname)] = value; } }

        public string cp_housename { get { return this.GetAttributeValue<string>(nameof(cp_housename)); } set { this.Attributes[nameof(cp_housename)] = value; } }
        public string cp_housenumber { get { return this.GetAttributeValue<string>(nameof(cp_housenumber)); } set { this.Attributes[nameof(cp_housenumber)] = value; } }
        public string cp_district { get { return this.GetAttributeValue<string>(nameof(cp_district)); } set { this.Attributes[nameof(cp_district)] = value; } }
        public string address1_line1 { get { return this.GetAttributeValue<string>(nameof(address1_line1)); } set { this.Attributes[nameof(address1_line1)] = value; } }
        public string address1_city { get { return this.GetAttributeValue<string>(nameof(address1_city)); } set { this.Attributes[nameof(address1_city)] = value; } }
        public string address1_county { get { return this.GetAttributeValue<string>(nameof(address1_county)); } set { this.Attributes[nameof(address1_county)] = value; } }
        public string address1_country { get { return this.GetAttributeValue<string>(nameof(address1_country)); } set { this.Attributes[nameof(address1_country)] = value; } }
        public string address1_postalcode { get { return this.GetAttributeValue<string>(nameof(address1_postalcode)); } set { this.Attributes[nameof(address1_postalcode)] = value; } }
        public string address1_telephone1 { get { return this.GetAttributeValue<string>(nameof(address1_telephone1)); } set { this.Attributes[nameof(address1_telephone1)] = value; } }
        public string mobilephone { get { return this.GetAttributeValue<string>(nameof(mobilephone)); } set { this.Attributes[nameof(mobilephone)] = value; } }
        public string company { get { return this.GetAttributeValue<string>(nameof(company)); } set { this.Attributes[nameof(company)] = value; } }
        public string emailaddress1 { get { return this.GetAttributeValue<string>(nameof(emailaddress1)); } set { this.Attributes[nameof(emailaddress1)] = value; } }
        public string cp_additionalname { get { return this.GetAttributeValue<string>(nameof(cp_additionalname)); } set { this.Attributes[nameof(cp_additionalname)] = value; } }
        public string cp_birthplace { get { return this.GetAttributeValue<string>(nameof(cp_birthplace)); } set { this.Attributes[nameof(cp_birthplace)] = value; } }
        public DateTime? birthdate { get { return this.GetAttributeValue<DateTime?>(nameof(birthdate)); } set { this.Attributes[nameof(birthdate)] = value; } }
        public EntityReference cp_contacttitle { get { return this.GetAttributeValue<EntityReference>(nameof(cp_contacttitle)); } set { this.Attributes[nameof(cp_contacttitle)] = value; } }
        public EntityReference cp_gender { get { return this.GetAttributeValue<EntityReference>(nameof(cp_gender)); } set { this.Attributes[nameof(cp_gender)] = value; } }
        public EntityReference cp_officerdefinedethnicity { get { return this.GetAttributeValue<EntityReference>(nameof(cp_officerdefinedethnicity)); } set { this.Attributes[nameof(cp_officerdefinedethnicity)] = value; } }
        public EntityReference cp_contactpreferredmethodofcontact { get { return this.GetAttributeValue<EntityReference>(nameof(cp_contactpreferredmethodofcontact)); } set { this.Attributes[nameof(cp_contactpreferredmethodofcontact)] = value; } }
        public OptionSetValue familystatuscode { get { return this.GetAttributeValue<OptionSetValue>(nameof(familystatuscode)); } set { this.Attributes[nameof(familystatuscode)] = value; } }

        public OptionSetValue cp_retired { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_retired)); } set { this.Attributes[nameof(cp_retired)] = value; } }
        public string cp_otheroccupation { get { return this.GetAttributeValue<string>(nameof(cp_otheroccupation)); } set { this.Attributes[nameof(cp_otheroccupation)] = value; } }

    }
}
