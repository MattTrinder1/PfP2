using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
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

        public OptionSetValue cp_reportingofficer { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_reportingofficer)); } set { this.Attributes[nameof(cp_reportingofficer)] = value; } }
        public OptionSetValue cp_singleofficerevent { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_singleofficerevent)); } set { this.Attributes[nameof(cp_singleofficerevent)] = value; } }
        public EntityReference cp_additionalofficer { get { return this.GetAttributeValue<EntityReference>(nameof(cp_additionalofficer)); } set { this.Attributes[nameof(cp_additionalofficer)] = value; } }

    }
}
