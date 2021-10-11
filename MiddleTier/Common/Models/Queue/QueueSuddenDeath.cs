using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Common.Models.Queue

{
    public class QueueSuddenDeath
    {
        public List<QueueContact> Contacts { get; set; }
        public List<QueueSuddenDeathProperty> Properties { get; set; }

        public string Id{ get; set; }

        
        public string EnteredBy { get; set; }

        [JsonPropertyName("Action of next of Kin")]
        public string ActionOfNextOfKin { get; set; }

        public string AdditionalNotes { get; set; }

        public string Additionalofficer { get; set; }

        public string Additionalofficerid { get; set; }

        [JsonPropertyName("Address sudden death")]
        public string AddressSuddenDeath { get; set; }

        [JsonPropertyName("Body found By")]
        public string BodyFoundBy { get; set; }

        [JsonPropertyName("Burial Or Cremation")]
        public string BurialOrCremation { get; set; }

        [JsonPropertyName("Burial Or Cremation name")]
        public string BurialOrCremationName { get; set; }

        public string CID_Attending { get; set; }

        public string CIDattended { get; set; }

        public string CIDcsiselectid { get; set; }

        public string CIDcsiselectname { get; set; }

        public string Circumstances { get; set; }

        public string CircumstancesAdditionalInfo { get; set; }

        public string Clothing { get; set; }

        [JsonPropertyName("Country sudden death")]
        public string CountrySuddenDeath { get; set; }

        [JsonPropertyName("County sudden death")]
        public string CountySuddenDeath { get; set; }

        [JsonPropertyName("Date body found")]
        public string DateBodyFound { get; set; }

        [JsonPropertyName("Date fact confirmed")]
        public string DateFactConfirmed { get; set; }

        public string DateLastSeenAlive { get; set; }

        [JsonPropertyName("Day of Death")]
        public string DayOfDeath { get; set; }

        [JsonPropertyName("Death diagnosed by")]
        public string DeathDiagnosedBy { get; set; }

        public string DeathInCustody { get; set; }

        public string DeathInHealthCare { get; set; }

        public string DeathInHospital { get; set; }

        public string DeceasedSmoker { get; set; }

        [JsonPropertyName("District sudden death")]
        public string DistrictSuddenDeath { get; set; }

        public string Dols { get; set; }

        public string Enteredby { get; set; }

        [JsonPropertyName("Family Liasion officer")]
        public string FamilyLiasionOfficer { get; set; }

        [JsonPropertyName("Formal Identification steps")]
        public string FormalIdentificationSteps { get; set; }

        [JsonPropertyName("Formal identification")]
        public string FormalIdentification { get; set; }

        public string GPDistrict { get; set; }

        public string GPaddress { get; set; }

        public string GPcounty { get; set; }

        public string GPdetailsknown { get; set; }

        public string GPname { get; set; }

        public string GPphonenumber { get; set; }

        public string GPpostcode { get; set; }

        public string GPstreet { get; set; }

        public string GPsurgery { get; set; }

        public string GPtown { get; set; }

        public string HouseTemperature { get; set; }


        public string HouseTemperaturename { get; set; }

        [JsonPropertyName("Housename sudden death")]
        public string HousenameSuddenDeath { get; set; }

        [JsonPropertyName("Houseno Sudden death")]
        public string HousenoSuddendeath { get; set; }

        public string IdentifierContactKey { get; set; }

        [JsonPropertyName("Incident date")]
        public string IncidentDate{ get; set; }

        [JsonPropertyName("Incident number")]
        public string IncidentNumber { get; set; }

        [JsonPropertyName("Incident suffix")]
        public string IncidentSuffix{ get; set; }

        public string Key { get; set; }

        public string LastSeenAliveBy { get; set; }

        [JsonPropertyName("Latitude Sudden Death")]
        public string LatitudeSuddenDeath { get; set; }

        [JsonPropertyName("Liasion officer phone")]
        public string LiasionOfficerPhone { get; set; }

        [JsonPropertyName("Longtitude Sudden Death")]
        public string LongtitudeSuddenDeath { get; set; }

        public string MarksBruises { get; set; }

        public string MedicalHistoryDiagnosisanmedication { get; set; }

        public string MedicalHistoryPastHistory { get; set; }

        public string MedicalHistoryReasonForVisit { get; set; }

        public string MedicalHistoryRiskFactors { get; set; }

        public string MedicalHistorylastvisitdate { get; set; }

        [JsonPropertyName("NPT sudden death")]
        public string NPTSuddenDeath { get; set; }

        [JsonPropertyName("NPT sudden death name")]
        public string NPTSuddenDeathName { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("Next of kin informed")]
        public string NextOfKinInformed { get; set; }

        [JsonPropertyName("Next of kin way of info")]
        public string NextOfKinWayOfInfo { get; set; }

        public string PNCId { get; set; }

        public string Pdetailsknown { get; set; }

        public string PhotoCircumstancesBlobName { get; set; }

        public string PhotoSuicideNoteBlobName { get; set; }
        public string identificationSignatureBlobName { get; set; }

        public string PhotosTakenbyCID { get; set; }

        public string PhysicalPosition { get; set; }

        [JsonPropertyName("Place of death desc")]
        public string PlaceOfDeathDesc { get; set; }

        public string PoliceContactPriorToDeath { get; set; }

        public string PrimaryOfficer { get; set; }

        [JsonPropertyName("Removed to")]
        public string RemovedTo { get; set; }

        public string SecureHouse { get; set; }

        public string SecureHousename { get; set; }

        [JsonPropertyName("Single Officer")]
        public string SingleOfficer{ get; set; }

        public string SuicideNoteLeft { get; set; }

        public string SuspectSuicide { get; set; }

        public string TelephoneLastSeenAliveBy { get; set; }

        [JsonPropertyName("Time Body Found")]
        public string TimeBodyFound { get; set; }

        [JsonPropertyName("Time fact confirmed")]
        public string TimeFactConfirmed { get; set; }

        public string TimeLastSeenalive { get; set; }

        [JsonPropertyName("Town sudden death")]
        public string TownSuddenDeath { get; set; }

        [JsonPropertyName("Undertaker funeral")]
        public string UndertakerFuneral { get; set; }

        [JsonPropertyName("Undertaker removing body")]
        public string UndertakerRemovingBody { get; set; }

        public string WhereLastSeenAlive { get; set; }

        public string WorkRelatedDeath { get; set; }

        [JsonPropertyName("pcode sudden death")]
        public string PcodeSuddenDeath { get; set; }

        public string photoCircumstancesInternalBlobId { get; set; }

        public string photoSuicideNoteInternalBlobId { get; set; }
       
    }
}
