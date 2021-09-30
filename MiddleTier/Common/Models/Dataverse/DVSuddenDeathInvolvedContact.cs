using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_suddendeathinvolvedcontact")]
    public class DVSuddenDeathInvolvedContact : DVEntityBase
    {
        public DVSuddenDeathInvolvedContact()
        {
            this.LogicalName = "cp_suddendeathinvolvedcontact";
        }


        public EntityReference cp_contact { get { return this.GetAttributeValue<EntityReference>(nameof(cp_contact)); } set { this.Attributes[nameof(cp_contact)] = value; } }
        public EntityReference cp_contactroletype { get { return this.GetAttributeValue<EntityReference>(nameof(cp_contactroletype)); } set { this.Attributes[nameof(cp_contactroletype)] = value; } }
        public EntityReference cp_suddendeath { get { return this.GetAttributeValue<EntityReference>(nameof(cp_suddendeath)); } set { this.Attributes[nameof(cp_suddendeath)] = value; } }


        public string cp_relationshiptodeceased { get { return this.GetAttributeValue<string>(nameof(cp_relationshiptodeceased)); } set { this.Attributes[nameof(cp_relationshiptodeceased)] = value; } }
        public string cp_relationshiptodeceasedduration { get { return this.GetAttributeValue<string>(nameof(cp_relationshiptodeceasedduration)); } set { this.Attributes[nameof(cp_relationshiptodeceasedduration)] = value; } }
        public string cp_involvedcontactname { get { return this.GetAttributeValue<string>(nameof(cp_involvedcontactname)); } set { this.Attributes[nameof(cp_involvedcontactname)] = value; } }

    }
}
