using Microsoft.Xrm.Sdk;
using System;

namespace Common.Models.Dataverse
{
    public class DVSuddenDeath : Entity
    {
        public DVSuddenDeath()
        {
            this.LogicalName = "cp_suddendeath";
        }

        public Guid? cp_suddendeathid { get { return base.Id; } set { base.Id = value.Value; } }

        
        public EntityReference cp_incident { get { return this.GetAttributeValue<EntityReference>(nameof(cp_incident)); } set { this.Attributes[nameof(cp_incident)] = value; } }

        public string cp_arealastseenalive { get { return this.GetAttributeValue<string>(nameof(cp_arealastseenalive)); } set { this.Attributes[nameof(cp_arealastseenalive)] = value; } }
        public string cp_nextofkininformedmethod { get { return this.GetAttributeValue<string>(nameof(cp_nextofkininformedmethod)); } set { this.Attributes[nameof(cp_nextofkininformedmethod)] = value; } }
        public string cp_deathdiagnosedby { get { return this.GetAttributeValue<string>(nameof(cp_deathdiagnosedby)); } set { this.Attributes[nameof(cp_deathdiagnosedby)] = value; } }
        public string cp_identificationlocation { get { return this.GetAttributeValue<string>(nameof(cp_identificationlocation)); } set { this.Attributes[nameof(cp_identificationlocation)] = value; } }
        public string cp_undertakerarrangingfuneral { get { return this.GetAttributeValue<string>(nameof(cp_undertakerarrangingfuneral)); } set { this.Attributes[nameof(cp_undertakerarrangingfuneral)] = value; } }
        public string cp_sdfamilylaisonofficer { get { return this.GetAttributeValue<string>(nameof(cp_sdfamilylaisonofficer)); } set { this.Attributes[nameof(cp_sdfamilylaisonofficer)] = value; } }
        public string cp_sdlastseenaliveby { get { return this.GetAttributeValue<string>(nameof(cp_sdlastseenaliveby)); } set { this.Attributes[nameof(cp_sdlastseenaliveby)] = value; } }
        public string cp_nextofkinactiontoinform { get { return this.GetAttributeValue<string>(nameof(cp_nextofkinactiontoinform)); } set { this.Attributes[nameof(cp_nextofkinactiontoinform)] = value; } }
        public string cp_certifiedby { get { return this.GetAttributeValue<string>(nameof(cp_certifiedby)); } set { this.Attributes[nameof(cp_certifiedby)] = value; } }
        public string cp_placeofdeath { get { return this.GetAttributeValue<string>(nameof(cp_placeofdeath)); } set { this.Attributes[nameof(cp_placeofdeath)] = value; } }
        public string cp_bodyremovedto { get { return this.GetAttributeValue<string>(nameof(cp_bodyremovedto)); } set { this.Attributes[nameof(cp_bodyremovedto)] = value; } }
        public string cp_bodyfoundby { get { return this.GetAttributeValue<string>(nameof(cp_bodyfoundby)); } set { this.Attributes[nameof(cp_bodyfoundby)] = value; } }
        public string cp_bodylabel { get { return this.GetAttributeValue<string>(nameof(cp_bodylabel)); } set { this.Attributes[nameof(cp_bodylabel)] = value; } }


    }
}
