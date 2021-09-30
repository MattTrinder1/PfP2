using System;
using System.Collections.Generic;

namespace Common.Models.Business
{
    public class SuddenDeath : EntityBase
    {
        //text fields
        public List<Contact> Contacts { get; set; }
        public List<SuddenDeathProperty> Properties { get; set; }
        public string EnteredBy { get; set; }
        public string AreaLastSeenAlive { get; set; }
        public string NextOfKinInformedMethod { get; set; }
        public string DeathDiagnosedBy { get; set; }
        public string UndertakerArrangingFuneral { get; set; }
        public string FamilyLiaisonOfficer { get; set; }
        public string LastSeenAliveBy { get; set; }
        public string NextOfKinActionToInform { get; set; }
        public string CertifiedBy { get; set; }
        public string PlaceOfDeath { get; set; }
        public string BodyRemovedTo { get; set; }
        public string BodyFoundBy { get; set; }
        public string BodyLabel { get; set; }
        public string SupervisorNotes { get; set; }
        public string MarkBruises { get; set; }
        public string FormalIdentificationSteps { get; set; }
        public string BodyPhysicalPosition { get; set; }
        public string InquestVerdict { get; set; }
        public string ClothingGeneralCondition { get; set; }
        public string ApprovalStatusReason { get; set; }
        public string AdditionalDetails { get; set; }
        public string AdditionalNotes { get; set; }
        public string CauseOfDeath { get; set; }
        public string CoronerOfficeNotes { get; set; }
        public string Circumstances { get; set; }
        public string InquestLocation { get; set; }
        public string HouseNameSuddenDeath { get; set; }
        public string HouseNoSuddenDeath { get; set; }
        public string CountySuddenDeath { get; set; }
        public string CountrySuddenDeath { get; set; }
        public string TownSuddenDeath { get; set; }
        public string AddressSuddenDeath { get; set; }
        public string DistrictSuddenDeath { get; set; }
        public double LatitudeSuddenDeath { get; set; }
        public double LongtitudeSuddenDeath { get; set; }
        public DateTime? IdentificationSignedOn { get; set; }
        public DateTime? DatetimeDeathConfirmed { get; set; }
        public DateTime? DatetimeBodyFound { get; set; }
        public DateTime? DatetimeLastSeenAlive { get; set; }
        public DateTime? AllPropertiesSignedOn { get; set; }
        public DateTime? InquestDate { get; set; }

        public string CIDCSIAttended { get; set; }
        public string NextOfKinInformed { get; set; }
        public string CIDCSIPhotosTaken { get; set; }
        public string BodyIdentified { get; set; }
        public string FormalIdentification { get; set; }
        public string UndertakerRemovingBody { get; set; }
        public string SuspectSuicide { get; set; }
        public string DeathCertificateIssued { get; set; }

        public string PhotoCircumstances { get; set; }
        public string PhotoSuicideNote { get; set; }
        public string IdentificationSignature { get; set; }

        public int DeceasedAge { get; set; }

        public string DOLS { get; set; }
        public string SuicideNoteLeft { get; set; }
        public string IsSubmitted { get; set; }
        public string Smoker { get; set; }
        public string PoliceInvolvementPriorDeath { get; set; }
        public string DeathInHealthCare { get; set; }
        public string WorkRelatedDeath { get; set; }
        public string DeathInCustody { get; set; }
        public string ApprovalStatus { get; set; }
        public string DeathInHospital { get; set; }

        public Guid? HouseTemperature { get; set; }
        public Guid? SecondPointOfContact { get; set; }
        public Guid? DeathType { get; set; }
        public Guid? Spouse { get; set; }
        public Guid? BurialCremation { get; set; }
        public Guid? NextOfKin { get; set; }
        public Guid? TPA { get; set; }
        public Guid? ParentGuardian { get; set; }
        public Guid? CertifiedRole { get; set; }
        public Guid? Deceased { get; set; }
        public Guid? HouseSecure { get; set; }
        public Guid? IdentifiedBy { get; set; }
        public Guid? InvestigationStatus { get; set; }
        public List<Guid> CIDCSISelectedIds { get; set; }


        public string GPDistrict { get; set; }
        public string GPAddress { get; set; }
        public string GPCounty { get; set; }
        public string GPName { get; set; }
        public string GPPhoneNumber { get; set; }
        public string GPPostcode { get; set; }
        public string GPStreet { get; set; }
        public string GPSurgery { get; set; }
        public string GPTown { get; set; }


        public string MedicalHistoryDiagnosisAnMedication { get; set; }
        public string MedicalHistoryPastHistory { get; set; }
        public string MedicalHistoryReasonForVisit { get; set; }
        public string MedicalHistoryRiskFactors { get; set; }
        public string MedicalHistoryLastVisitDate { get; set; }




    }
}
