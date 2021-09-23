using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace API.Models.PNB
{
    [EntityLogicalName("cp_incident")]
    public class DVIncident : DVEntityBase
    {
        public DVIncident()
        {
            this.LogicalName = "cp_incident";
        }

        public Guid? cp_incidentid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_incidentnumber { get { return this.GetAttributeValue<string>(nameof(cp_incidentnumber)); } set { this.Attributes[nameof(cp_incidentnumber)] = value; } }

        public DateTime? cp_incidentdate { get { return this.GetAttributeValue<DateTime?>(nameof(cp_incidentdate)); } set { this.Attributes[nameof(cp_incidentdate)] = value; } }

        public EntityReference cp_incidenttype { get { return this.GetAttributeValue<EntityReference>(nameof(cp_incidenttype)); } set { this.Attributes[nameof(cp_incidenttype)] = value; } }
        public EntityReference cp_incidentlocation { get { return this.GetAttributeValue<EntityReference>(nameof(cp_incidentlocation)); } set { this.Attributes[nameof(cp_incidentlocation)] = value; } }

        public EntityReference cp_enteredby { get { return this.GetAttributeValue<EntityReference>(nameof(cp_enteredby)); } set { this.Attributes[nameof(cp_enteredby)] = value; } }

    }
}
