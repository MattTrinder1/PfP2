using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace API.Models.Dataverse
{
    [EntityLogicalName("cp_medicalhistory")]
    public class DVMedicalHistory : DVEntityBase
    {
        public DVMedicalHistory()
        {
            this.LogicalName = "cp_medicalhistory";
        }


        public string cp_medicalhistoryname { get { return this.GetAttributeValue<string>(nameof(cp_medicalhistoryname)); } set { this.Attributes[nameof(cp_medicalhistoryname)] = value; } }
        public EntityReference cp_contact { get { return this.GetAttributeValue<EntityReference>(nameof(cp_contact)); } set { this.Attributes[nameof(cp_contact)] = value; } }
        public EntityReference cp_gp { get { return this.GetAttributeValue<EntityReference>(nameof(cp_gp)); } set { this.Attributes[nameof(cp_gp)] = value; } }
        public DateTime? cp_gpvisitdate { get { return this.GetAttributeValue<DateTime?>(nameof(cp_gpvisitdate)); } set { this.Attributes[nameof(cp_gpvisitdate)] = value; } }
        public string cp_diagnosismedicationprescribed { get { return this.GetAttributeValue<string>(nameof(cp_diagnosismedicationprescribed)); } set { this.Attributes[nameof(cp_diagnosismedicationprescribed)] = value; } }
        public string cp_knownriskfactors { get { return this.GetAttributeValue<string>(nameof(cp_knownriskfactors)); } set { this.Attributes[nameof(cp_knownriskfactors)] = value; } }
        public string cp_pastmedicalhistory { get { return this.GetAttributeValue<string>(nameof(cp_pastmedicalhistory)); } set { this.Attributes[nameof(cp_pastmedicalhistory)] = value; } }
        public string cp_reasonforvisit { get { return this.GetAttributeValue<string>(nameof(cp_reasonforvisit)); } set { this.Attributes[nameof(cp_reasonforvisit)] = value; } }

    }
}
