using NDIXML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDIApiWrapper.Models
{
    public class PersonQueryResult
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string PNCId { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Address1 { get; internal set; }
        public string Address2 { get; internal set; }
        public string Address3 { get; internal set; }
        public string DriverNumber { get; internal set; }
        public string Forenames { get; internal set; }
        public string Surname { get; internal set; }
        public bool Selected { get; set; }
        public string Postcode { get; set; }
        public string ResultFrom { get; set; }
    }

    public class PersonWrapper
    {
        public PersonWrapper()
        {
            Records = new List<PersonQueryResult>();
        }
        public List<PersonQueryResult> Records { get; set; }
    }

    public class Person
    {
        public Person()
        {
            MarkScars = new List<MarkScar>();
            Alerts = new List<Alert>();
            WarningSignals = new List<WarningSignal>();
            AliasNames = new List<AliasName>();
            AliasDOBs = new List<AliasDateOfBirth>();
            InformationMarkers = new List<InformationMarker>();
            Nicknames = new List<Nickname>();
            Addresses = new List<Address>();
            Descriptions = new List<Description>();
            Offences = new List<Offence>();
            DisposalSummaries = new List<DisposalSummary>();
            Addresses = new List<Address>();
            BailConditons = new List<BailCondition>();
            WantedMissings = new List<WantedMissing>();
            OperationalInfos = new List<OperationalInfo>();
            Disqualifieds = new List<Disqualified>();
            OtherDetails = new List<OtherDetail>();
            DocumentTrail = new List<Document>();
            StopsMarkers = new List<StopMarker>();
            FullEntitlement = new List<Entitlement>();
            ProvisionalEntitlement = new List<Entitlement>();
            DrivingSummary = new DrivingSummary();
            Endorsements = new List<Endorsement>();
            CrossRefs = new List<CrossRef>();
            Unclaimeds = new List<Unclaimed>();
        }
        public string StatusSum { get; internal set; }
        public bool Selected { get; set; }
        public string FileName { get; set; }
        public string WarningSignalsText { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string CRONumber { get; set; }
        public string FileDOB { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public string Birthplace { get; set; }
        public string Nationality { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string AddressDate { get; set; }

        public string PostCode { get; set; }
        public string DriverNumber { get; set; }

        public string PNCId { get; set; }

        public List<MarkScar> MarkScars { get; set; }
        public List<Alert> Alerts { get; set; }
        public List<WarningSignal> WarningSignals { get; set; }
        public List<AliasName> AliasNames { get; set; }
        public List<AliasDateOfBirth> AliasDOBs { get; set; }
        public List<InformationMarker> InformationMarkers { get; set; }
        public List<Offence> Offences { get; set; }
        public List<Address> Addresses { get; set; }
        public List<Nickname> Nicknames { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<DisposalSummary> DisposalSummaries { get; set; }
        public List<BailCondition> BailConditons { get; set; }
        public List<WantedMissing> WantedMissings { get; set; }
        public List<OperationalInfo> OperationalInfos { get; set; }
        public List<Disqualified> Disqualifieds { get; set; }
        public List<OtherDetail> OtherDetails { get; set; }

        public List<Document> DocumentTrail { get; set; }
        public List<StopMarker> StopsMarkers { get; set; }

        public List<Entitlement> FullEntitlement { get; set; }
        public List<Entitlement> ProvisionalEntitlement { get; set; }
        public List<Endorsement> Endorsements { get; set; }
        public List<CrossRef> CrossRefs { get; set; }
        public List<Unclaimed> Unclaimeds { get; set; }
        public DrivingSummary DrivingSummary { get; set; }
        public string LicenceStatus { get; internal set; }
        public string Disqualification { get; internal set; }
        public string LicenceType { get; internal set; }
        public string LicenceIssue { get; internal set; }
        public string CounterPartIssue { get; internal set; }
        public string CommencementDate { get; internal set; }
        public string ExpiryDate { get; internal set; }
        public string Title { get; internal set; }
    }

    public class CrossRef
    {
        public string Driver { get; set; }
        public string Date { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }

    }
    public class Unclaimed
    {
        public string Category { get; set; }
        public string Harm { get; set; }
        public string Date { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string Restriction1 { get; set; }
        public string Restriction2 { get; set; }
        public string Restriction3 { get; set; }
        public string Restriction4 { get; set; }
        public string EU3D { get; set; }

    }

    public class  Endorsement
    {
        public string ConvictionCourt { get; set; }
        public string ConvictionDate { get; set; }
        public string OffenceCode { get; set; }
        public string OffenceDate { get; set; }
        public string Fine { get; set; }
        public string Points { get; set; }
        public string DisqPeriod { get; set; }
        public string OtherSentence { get; set; }
        public string SuspendSentence { get; set; }
        public string DateDisqRemoved { get; set; }
        public string DTTPorDPS { get; set; }
        public string SentencingCourt { get; set; }
        public string SentencingDate { get; set; }
        public string DisqPendAppeal { get; set; }
        public string AppealDate { get; set; }
        public string DisqReimposed { get; set; }
        public string AppealCourt { get; set; }
        public string RehabReduction { get; set; }
        public string DisqPendSentence { get; set; }








    }

    public class DrivingSummary
    {
        public string Disqualified { get; set; }
        public string DrinkDrug { get; set; }
        public string Other { get; set; }
        public string  Points { get; set; }
    }

    public class Entitlement
    {
        public string Category { get; set; }
        public string From { get; set; }
        public string Until { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string Restriction1 { get; set; }
        public string Restriction2 { get; set; }
        public string Restriction3 { get; set; }
        public string Restriction4 { get; set; }
        public string EU3D { get; set; }
    }

    public class Document
    {
        public string DocumentRef { get; set; }
        public string Date { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }

    }
    public class StopMarker
    {
        public string Description { get; set; }

    }

    public class Nickname
    {
        public string Date { get; internal set; }
        public string Name { get; internal set; }
        public string Owner { get; internal set; }
    }

    public class Description
    {
        public string Accent { get; internal set; }
        public string BirthDay { get; internal set; }
        public string BirthMonth { get; internal set; }

        public string BirthYear { get; internal set; }
        public string Build { get; internal set; }
        public string POB { get; internal set; }
        public string Colour { get; internal set; }
        public string DriverNumber1 { get; internal set; }
        public string DriverNumber2 { get; internal set; }
        public string DriverNumber3 { get; internal set; }
        public string EACode { get; internal set; }
        public string EyeColour1 { get; internal set; }
        public string EyeColour2EyeColour2 { get; internal set; }
        public string FacialHair1Colour { get; internal set; }
        public string FacialHair1Colour2 { get; internal set; }
        public string FacialHair1DG { get; internal set; }
        public string FacialHair1Features { get; internal set; }
        public string FacialHair1Type { get; internal set; }
        public string FacialHair2Colour { get; internal set; }
        public string FacialHair2Colour2 { get; internal set; }
        public string FacialHair2DG { get; internal set; }
        public string FacialHair2Features { get; internal set; }
        public string FacialHair2Type { get; internal set; }
        public string FacialHair3Colour { get; internal set; }
        public string FacialHair3Colour2 { get; internal set; }
        public string FacialHair3DG { get; internal set; }
        public string FacialHair3Features { get; internal set; }
        public string FacialHair3Type { get; internal set; }
        public string FileName { get; internal set; }
        public string Glasses { get; internal set; }
        public string HairColour { get; internal set; }
        public string HairColour2 { get; internal set; }
        public string HairDG { get; internal set; }
        public string HairFeatures1 { get; internal set; }
        public string HairFeatures2 { get; internal set; }
        public string HairType { get; internal set; }
        public string Handedness { get; internal set; }
        public string Height { get; internal set; }
        public string MSA1 { get; internal set; }
        public string MSA2 { get; internal set; }
        public string MSA3 { get; internal set; }
        public string MSA4  { get; internal set; }
    public string Nationality1 { get; internal set; }
    public string Nationality2 { get; internal set; }
    public string Nationality3 { get; internal set; }
    public string Sex { get; internal set; }
    public string ShoeSize { get; internal set; }
}

    public class DisposalSummary
    {
        public DisposalSummary()
        {
            Offences = new List<Offence>();
        }

        public string DSExtraTextCount { get; internal set; }
        public NEMenuDSExtraText[] DSExtraTextList { get; internal set; }
        public string DSOffenceCount { get; internal set; }
        public string FirstValue { get; internal set; }
        public string LastValue { get; internal set; }
        public string TotalCautions { get; internal set; }
        public string TotalConvictions { get; internal set; }
        public string TotalDeport { get; internal set; }
        public string TotalNFA { get; internal set; }
        public string TotalNINonCourtDisposals { get; internal set; }
        public string TotalNonConvictions { get; internal set; }
        public string TotalNotGuilty { get; internal set; }
        public string TotalPenaltyNotices { get; internal set; }
        public string TotalRefer { get; internal set; }
        public string TotalReprimands { get; internal set; }
        public string TotalUnobtainable { get; internal set; }
        public string TotalWarnings { get; internal set; }

        public List<Offence> Offences { get; set; }
    }



    public class BailCondition
    {
        public BailCondition()
        {
            Conditions = new List<BailConditionCondition>();
        }

        public string Remand { get; internal set; }
        public string CountValue { get; internal set; }
        public object Owner { get; internal set; }
        public string BailAddress { get; internal set; }
        public object Arrest { get; internal set; }
        public object Alert { get; internal set; }
        public string NextValue { get; internal set; }

        public List<BailConditionCondition> Conditions { get; set; }
    }

    public class WantedMissing
    {
        public string CasePapers   { get; internal set; }
            public string Class  { get; internal set; }
            //public string Date NextValue { get; internal set; }
            public string Count  { get; internal set; }
            public string DateOn  { get; internal set; }
            public string DateToDateTo { get; internal set; }
            public string EndDate { get; internal set; }
            public string For { get; internal set; }
            public string FSRef { get; internal set; }
            public string IssuedAt { get; internal set; }
            public string Line13  { get; internal set; }
            public string Line14 { get; internal set; }
            public string Line15 { get; internal set; }
            public string Line16 { get; internal set; }
            public string Line17 { get; internal set; }
            public string Line18 { get; internal set; }
            public string Line19 { get; internal set; }
            public string Line20 { get; internal set; }
            public string Line21 { get; internal set; }
            public string Line22 { get; internal set; }
            public string Location { get; internal set; }
            public string PageRef { get; internal set; }
            public string Power { get; internal set; }
            public string Reported { get; internal set; }
            public string SACountry { get; internal set; }
            public string SACreated { get; internal set; }
            public string SADOB { get; internal set; }
            public string SAEAWText { get; internal set; }
            public string SAExpires { get; internal set; }
            public string SAIdentMark1 { get; internal set; }
            public string SAIdentMark2 { get; internal set; }
            public string SAIdStatus { get; internal set; }
            public string SALastUpdated { get; internal set; }
            public string SALinked { get; internal set; }
            public string SAName { get; internal set; }
            public string SANationality1{ get; internal set; }
            public string SANationality2 { get; internal set; }
            public string SANationality3 { get; internal set; }
            public string SAOffenceCategory{ get; internal set; }
            public string SARiskStatus { get; internal set; }
            public string SAStatus { get; internal set; }
            public string SAText1 { get; internal set; }
            public string SAText2 { get; internal set; }
            public string SAText3{ get; internal set; }
            public string SAText4 { get; internal set; }
            public string SAText5 { get; internal set; }
            public string SAText6{ get; internal set; }
            public string SAText7 { get; internal set; }
            public string SAText8 { get; internal set; }
            public string SAText9 { get; internal set; }
            public string SAText10 { get; internal set; }
            public string SAText11 { get; internal set; }
            public string SAText12 { get; internal set; }
            public string SATRAliasCount { get; internal set; }
            //public string Owner = menuWM.SATRAliasList.FieldData;
            public string SATRBirthplace1 { get; internal set; }
            public string SATRBirthplace2 { get; internal set; }
            public string SATRBirthplace3 { get; internal set; }
            public string SATRBirthplace5 { get; internal set; }
            public string SATRBirthplace4 { get; internal set; }
            public string SATRFilename { get; internal set; }
            public string SAType { get; internal set; }
            public string SISId { get; internal set; }
            public string Text1 { get; internal set; }
            public string Text2 { get; internal set; }
            public string Text3 { get; internal set; }
            public string WarrantBailIssuedAt  { get; internal set; }
            public string WarrantIssuedAt{ get; internal set; }
            public string Weed { get; internal set; }
    }
    public class OperationalInfo
    {
        public string ExpiryDate { get; internal set; }
        public string FSRef { get; internal set; }
        public string Power { get; internal set; }
        public string StartDate { get; internal set; }
        public string Type { get; internal set; }
    }

    public class Disqualified
    {
        public string Date { get; internal set; }
        public string Name { get; internal set; }
        public string FSRef { get; internal set; }
        public string Notes { get; internal set; }
        public string Sentence { get; internal set; }
        public string Text { get; internal set; }
    }

    public class OtherDetail
    {
        public string Owner { get; internal set; }
        public string Text { get; internal set; }
        public string Update { get; internal set; }
    }

    public class MarkScar
    {
        public string Type { get; set; }
        public string Keyword1 { get; set; }
        public string Keyword2 { get; set; }
        public string Keyword3 { get; set; }
        public string Text { get; set; }
        public string Detail { get; set; }
        public string Location { get; set; }

    }

    public class Alert
    {
        public string Text { get; set; }
    }

    public class WarningSignal
    {
        public string Warning { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string FSRef { get; set; }
        public string Updated { get; set; }
    }

    public class AliasName
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
    
    }

    public class AliasDateOfBirth
    {
        public string Date { get; set; }
        public string AliasDOB { get; set; }
        public string Owner { get; set; }
    }

        public class InformationMarker
    {
        public string Marker { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string FSRef { get; set; }
        public string Updated { get; set; }
    }

    public class BailConditionCondition
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }

    }

    public class Offence
    {
        public string OffenceCount { get; set; }
        public string OffenceDescription { get; set; }
        public string OffencePeriod { get; set; }
    }

    public class Address
    {
        public string Address1 { get; internal set; }
        public string Address2 { get; internal set; }
        public string Address3 { get; internal set; }
        public string Description { get; internal set; }
        public string Owner { get; internal set; }
        public string Updated { get; internal set; }
    }




}
