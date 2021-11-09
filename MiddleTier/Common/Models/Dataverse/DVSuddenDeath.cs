using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName ("cp_suddendeath")]
    public class DVSuddenDeath : DVEntityBase
    {
        public DVSuddenDeath()
        {
            this.LogicalName = "cp_suddendeath";
        }

        public string cp_name { get { return this.GetAttributeValue<string>(nameof(cp_name)); } set { this.Attributes[nameof(cp_name)] = value; } }

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
        public string cp_supervisornotes { get { return this.GetAttributeValue<string>(nameof(cp_supervisornotes)); } set { this.Attributes[nameof(cp_supervisornotes)] = value; } }
        public string cp_markbruises { get { return this.GetAttributeValue<string>(nameof(cp_markbruises)); } set { this.Attributes[nameof(cp_markbruises)] = value; } }
        public string cp_formalidentificationsteps { get { return this.GetAttributeValue<string>(nameof(cp_formalidentificationsteps)); } set { this.Attributes[nameof(cp_formalidentificationsteps)] = value; } }
        public string cp_bodyphysicalposition { get { return this.GetAttributeValue<string>(nameof(cp_bodyphysicalposition)); } set { this.Attributes[nameof(cp_bodyphysicalposition)] = value; } }
        public string cp_inquestverdict { get { return this.GetAttributeValue<string>(nameof(cp_inquestverdict)); } set { this.Attributes[nameof(cp_inquestverdict)] = value; } }
        public string cp_clothinggeneralcondition { get { return this.GetAttributeValue<string>(nameof(cp_clothinggeneralcondition)); } set { this.Attributes[nameof(cp_clothinggeneralcondition)] = value; } }
        public string cp_approvalstatusreason { get { return this.GetAttributeValue<string>(nameof(cp_approvalstatusreason)); } set { this.Attributes[nameof(cp_approvalstatusreason)] = value; } }
        public string cp_additionaldetails { get { return this.GetAttributeValue<string>(nameof(cp_additionaldetails)); } set { this.Attributes[nameof(cp_additionaldetails)] = value; } }
        public string cp_additionalnotes { get { return this.GetAttributeValue<string>(nameof(cp_additionalnotes)); } set { this.Attributes[nameof(cp_additionalnotes)] = value; } }
        public string cp_causeofdeath { get { return this.GetAttributeValue<string>(nameof(cp_causeofdeath)); } set { this.Attributes[nameof(cp_causeofdeath)] = value; } }
        public string cp_coronerofficenotes { get { return this.GetAttributeValue<string>(nameof(cp_coronerofficenotes)); } set { this.Attributes[nameof(cp_coronerofficenotes)] = value; } }
        public string cp_circumstances { get { return this.GetAttributeValue<string>(nameof(cp_circumstances)); } set { this.Attributes[nameof(cp_circumstances)] = value; } }
        public string cp_inquestlocation { get { return this.GetAttributeValue<string>(nameof(cp_inquestlocation)); } set { this.Attributes[nameof(cp_inquestlocation)] = value; } }



        public DateTime? cp_identificationsignedon { get { return this.GetAttributeValue<DateTime>(nameof(cp_identificationsignedon)); } set { this.Attributes[nameof(cp_identificationsignedon)] = value; } }
        public DateTime? cp_datetimedeathconfirmed{ get { return this.GetAttributeValue<DateTime>(nameof(cp_datetimedeathconfirmed)); } set { this.Attributes[nameof(cp_datetimedeathconfirmed)] = value; } }
        public DateTime? cp_datetimebodyfound { get { return this.GetAttributeValue<DateTime>(nameof(cp_datetimebodyfound)); } set { this.Attributes[nameof(cp_datetimebodyfound)] = value; } }
        public DateTime? cp_datetimelastseenalive { get { return this.GetAttributeValue<DateTime>(nameof(cp_datetimelastseenalive)); } set { this.Attributes[nameof(cp_datetimelastseenalive)] = value; } }
        public DateTime? cp_allpropertiessignedon { get { return this.GetAttributeValue<DateTime>(nameof(cp_allpropertiessignedon)); } set { this.Attributes[nameof(cp_allpropertiessignedon)] = value; } }
        public DateTime? cp_inquestdate { get { return this.GetAttributeValue<DateTime>(nameof(cp_inquestdate)); } set { this.Attributes[nameof(cp_inquestdate)] = value; } }
        
        public bool cp_cidcsiattended { get { return this.GetAttributeValue<bool>(nameof(cp_cidcsiattended)); } set { this.Attributes[nameof(cp_cidcsiattended)] = value; } }
        public bool cp_nextofkininformed { get { return this.GetAttributeValue<bool>(nameof(cp_nextofkininformed)); } set { this.Attributes[nameof(cp_nextofkininformed)] = value; } }
        public bool cp_cidcsiphotostaken { get { return this.GetAttributeValue<bool>(nameof(cp_cidcsiphotostaken)); } set { this.Attributes[nameof(cp_cidcsiphotostaken)] = value; } }
        public bool cp_bodyidentified { get { return this.GetAttributeValue<bool>(nameof(cp_bodyidentified)); } set { this.Attributes[nameof(cp_bodyidentified)] = value; } }
        public bool cp_formalidentification { get { return this.GetAttributeValue<bool>(nameof(cp_formalidentification)); } set { this.Attributes[nameof(cp_formalidentification)] = value; } }
        public bool cp_undertakerremovebody { get { return this.GetAttributeValue<bool>(nameof(cp_undertakerremovebody)); } set { this.Attributes[nameof(cp_undertakerremovebody)] = value; } }
        public bool cp_suspectsuicide { get { return this.GetAttributeValue<bool>(nameof(cp_suspectsuicide)); } set { this.Attributes[nameof(cp_suspectsuicide)] = value; } }
        public bool cp_deathcertificateissued { get { return this.GetAttributeValue<bool>(nameof(cp_deathcertificateissued)); } set { this.Attributes[nameof(cp_deathcertificateissued)] = value; } }


        public int cp_deceasedage { get { return this.GetAttributeValue<int>(nameof(cp_deceasedage)); } set { this.Attributes[nameof(cp_deceasedage)] = value; } }


        public OptionSetValue cp_dols { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_dols)); } set { this.Attributes[nameof(cp_dols)] = value; } }
        public OptionSetValue cp_issubmitted { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_issubmitted)); } set { this.Attributes[nameof(cp_issubmitted)] = value; } }
        public OptionSetValue cp_suicidenoteleft_new { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_suicidenoteleft_new)); } set { this.Attributes[nameof(cp_suicidenoteleft_new)] = value; } }
        public OptionSetValue cp_smoker { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_smoker)); } set { this.Attributes[nameof(cp_smoker)] = value; } }
        public OptionSetValue cp_policeinvolvementpriordeath { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_policeinvolvementpriordeath)); } set { this.Attributes[nameof(cp_policeinvolvementpriordeath)] = value; } }
        public OptionSetValue cp_deathinhealthcare { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_deathinhealthcare)); } set { this.Attributes[nameof(cp_deathinhealthcare)] = value; } }
        public OptionSetValue cp_workrelateddeath { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_workrelateddeath)); } set { this.Attributes[nameof(cp_workrelateddeath)] = value; } }
        public OptionSetValue cp_deathincustody { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_deathincustody)); } set { this.Attributes[nameof(cp_deathincustody)] = value; } }
        public OptionSetValue cp_approvalstatus { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_approvalstatus)); } set { this.Attributes[nameof(cp_approvalstatus)] = value; } }
        public OptionSetValue cp_deathinhospital { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_dols)); } set { this.Attributes[nameof(cp_deathinhospital)] = value; } }


        public EntityReference cp_housetemperature { get { return this.GetAttributeValue<EntityReference>(nameof(cp_housetemperature)); } set { this.Attributes[nameof(cp_housetemperature)] = value; } }
        public EntityReference cp_secondpointofcontact { get { return this.GetAttributeValue<EntityReference>(nameof(cp_secondpointofcontact)); } set { this.Attributes[nameof(cp_secondpointofcontact)] = value; } }
        public EntityReference cp_deathtype { get { return this.GetAttributeValue<EntityReference>(nameof(cp_deathtype)); } set { this.Attributes[nameof(cp_deathtype)] = value; } }
        public EntityReference cp_burialcremation { get { return this.GetAttributeValue<EntityReference>(nameof(cp_burialcremation)); } set { this.Attributes[nameof(cp_burialcremation)] = value; } }
        public EntityReference cp_tpa { get { return this.GetAttributeValue<EntityReference>(nameof(cp_tpa)); } set { this.Attributes[nameof(cp_tpa)] = value; } }
        public EntityReference cp_certifiedrole { get { return this.GetAttributeValue<EntityReference>(nameof(cp_certifiedrole)); } set { this.Attributes[nameof(cp_certifiedrole)] = value; } }
        public EntityReference cp_housesecure { get { return this.GetAttributeValue<EntityReference>(nameof(cp_housesecure)); } set { this.Attributes[nameof(cp_housesecure)] = value; } }
        public EntityReference cp_identifiedby { get { return this.GetAttributeValue<EntityReference>(nameof(cp_identifiedby)); } set { this.Attributes[nameof(cp_identifiedby)] = value; } }
        public EntityReference cp_investigationstatus { get { return this.GetAttributeValue<EntityReference>(nameof(cp_investigationstatus)); } set { this.Attributes[nameof(cp_investigationstatus)] = value; } }
        public EntityReference cp_deceased { get { return this.GetAttributeValue<EntityReference>(nameof(cp_deceased)); } set { this.Attributes[nameof(cp_deceased)] = value; } }

        
    }
}
