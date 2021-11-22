#if MOCK
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NDIApiWrapper.Models;
using NDIXML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NDIApiWrapper.Controllers
{
    [Route("api/person")]
    [ApiController]
    public class MockPersonController : ControllerBase
    {
        IConfiguration config;
        ILogger log;
        public MockPersonController(IConfiguration configuration,ILogger<MockPersonController> logger )
        {
            config = configuration;
            log = logger;

        }

        [HttpGet("getbydrivernumber/{driverNumber}")]
        public ActionResult<Person> GetByDriverNumber(string driverNumber,string reason)
        {
            log.LogInformation("GetByDL");
            var sw = new Stopwatch();
            sw.Start();


            var person = new Person();
            person.DriverNumber = driverNumber;
            person.FirstName = "MATT";
            person.LastName = "TRINDER";
            person.PostCode = "MK11 1DW";

            person.Endorsements.Add(new Endorsement());
            person.FullEntitlement.Add(new Entitlement());
            person.ProvisionalEntitlement.Add(new Entitlement());
            person.Unclaimeds.Add(new Unclaimed());
            person.StopsMarkers.Add(new StopMarker());
            person.DocumentTrail.Add(new Document());
            person.CrossRefs.Add(new CrossRef());

            return person;
        }


        [HttpGet("getbydl/{surname}/{forename1}")]
        public ActionResult<PersonWrapper> GetByDL(string surname, string forename1, string forename2, string dateofbirth, string gender, string ethnicity, string reason, string postcode)
        {

            var wrap = new PersonWrapper();

            var p1 = new PersonQueryResult();
            p1.Address1 = "100 WOLVERTON ROAD";
            p1.Address2 = "STONY STRATFORD";
            p1.Address3 = "MILTON KEYNES";
            p1.DateOfBirth = "10/03/1971";
            p1.DriverNumber = "TRIND703101M99SV";
            p1.Ethnicity = "W";
            p1.Forenames = "MATT";
            p1.Gender = "M";
            p1.Name = "MATT TRINDER";
            p1.PlaceOfBirth = "WALLINGFORD";
            p1.PNCId = "";
            p1.Postcode = "MK11 1DW";
            p1.ResultFrom = "DL";
            p1.Surname = "TRINDER";

            wrap.Records.Add(p1);

            var p2 = new PersonQueryResult();
            p2.Address1 = "101 SMITH STREET";
            p2.Address2 = "WOLVERTON";
            p2.Address3 = "MILTON KEYNES";
            p2.DateOfBirth = "11/04/1972";
            p2.DriverNumber = "JONES123456M99SV";
            p2.Ethnicity = "W";
            p2.Forenames = "WALT";
            p2.Gender = "M";
            p2.Name = "WALT JONES";
            p2.PlaceOfBirth = "LEEDS";
            p2.PNCId = "";
            p2.Postcode = "MK12 2DD";
            p2.ResultFrom = "DL";
            p2.Surname = "JONES";

            wrap.Records.Add(p2);

            return wrap;


        }

        private static PersonQueryResult GetPersonQueryResultFromDLResult(DLResult pers)
        {
            var res = new PersonQueryResult();
            res.Name = pers.Name.FieldData;
            res.Forenames = pers.Name.FieldData;
            res.Surname = pers.Name.FieldData;
            res.Address1 = pers.FullAddress1.FieldData;
            res.Address2 = pers.FullAddress2.FieldData;
            res.Address3 = pers.FullAddress3.FieldData;
            res.DateOfBirth = pers.Birth.FieldData;
            res.DriverNumber = pers.DriverNo.FieldData;
            res.Gender = pers.Sex.FieldData;
            res.PlaceOfBirth = pers.Birthplace.FieldData;
            res.Postcode = pers.PostCode.FieldData;
            res.ResultFrom = "DL";
            return res;
        }

        [HttpGet("getbypncid/{pncid}")]
        public ActionResult<Person> GetByPNCID(string pncId)
        {

            var sw = new Stopwatch();
            sw.Start();


            var person = new Person();
            person.PNCId = pncId;
            person.DriverNumber = "TRIND703101M99SV";
            person.FirstName = "MATT";
            person.LastName = "TRINDER";
            person.PostCode = "MK11 1DW";


            person.Endorsements.Add(new Endorsement());
            person.FullEntitlement.Add(new Entitlement());
            person.ProvisionalEntitlement.Add(new Entitlement());
            person.Unclaimeds.Add(new Unclaimed());
            person.StopsMarkers.Add(new StopMarker());
            person.DocumentTrail.Add(new Document());
            person.CrossRefs.Add(new CrossRef());
            

            person.MarkScars.Add(new MarkScar() { Type = "TATTOO", Detail = "SPIDER WEB", Location = "NECK" });
            person.InformationMarkers.Add(new InformationMarker());
            person.WarningSignals.Add(new WarningSignal());
            person.AliasNames.Add(new AliasName() { Name = "KEV SPIDER" });
            person.AliasDOBs.Add(new AliasDateOfBirth());
            person.Nicknames.Add(new Nickname() { Date = "01012001", Name = "SPIDER" });
            person.Descriptions.Add(new Description());
            person.DisposalSummaries.Add(new DisposalSummary());
            person.Addresses.Add(new Address());
            person.BailConditons.Add(new BailCondition());
            person.WantedMissings.Add(new WantedMissing());
            person.OperationalInfos.Add(new OperationalInfo());
            person.Disqualifieds.Add(new Disqualified());
            person.OtherDetails.Add(new OtherDetail());

            return person;

                
        }

        private void PopulateRelated<TMenu,TRecordType>(ConsoliDataWSSoapClient client, PNCScreen connectResponse,  NEResult res,string suffix,List<TRecordType> list,Func<TMenu,TRecordType> extractMethod)
        {
            var menuRes = client.NominalMenuParsed2(connectResponse.Session.SessionInfo, suffix);

            //special case for wantedmissing
            if (suffix == "WM")
            {
                suffix = "WM2";
            }

            var prop = typeof(PNCScreen).GetProperty("NEMenu" + suffix);

            var menuData = (TMenu)prop.GetValue(menuRes);

            if (menuData != null)
            {
                list.Add(extractMethod(menuData));
            }

            while (menuRes.Control.State != "NOMINAL")
            {
                menuRes = client.NominalMenuParsedMore(connectResponse.Session.SessionInfo);
                menuData = (TMenu)prop.GetValue(menuRes);
                if (menuData != null)
                {
                    list.Add(extractMethod(menuData));
                }
            }
            
        }
        
        private MarkScar GetMarkScarFromMenu(NEMenuMS menuMS)
        {
            var ms = new MarkScar();
            ms.Keyword1 = menuMS.Keyword1.FieldData;
            ms.Keyword2 = menuMS.Keyword2.FieldData;
            ms.Keyword3 = menuMS.Keyword3.FieldData;
            ms.Text = menuMS.Text.FieldData;
            ms.Type = menuMS.Type.FieldData;
            ms.Location = menuMS.Location.FieldData;
            ms.Detail = menuMS.Detail.FieldData;


            return ms;
        }

        private Nickname GetNicknameFromMenu(NEMenuNK menuNK)
        {
            var nn = new Nickname();
            nn.Date = menuNK.Date.FieldData;
            nn.Name = menuNK.Name.FieldData;
            nn.Owner = menuNK.Owner.FieldData;


            return nn;
        }
        private OtherDetail GetOtherDetailsFromMenu(NEMenuOD menuOD)
        {
            var od = new OtherDetail();
            od.Owner = menuOD.Owner.FieldData;
            od.Text = menuOD.Text.FieldData;
            od.Update = menuOD.Update.FieldData;


            return od;
        }
        private Disqualified GetDisqualifiedsFromMenu(NEMenuDD menuDD)
        {
            var dd = new Disqualified();
            dd.Date = menuDD.Court.FieldData;
            dd.Name = menuDD.DateFrom.FieldData;
            dd.FSRef = menuDD.FSRef.FieldData;
            dd.Notes = menuDD.Notes.FieldData;
            dd.Sentence = menuDD.Sentence.FieldData;
            dd.Text = menuDD.Text.FieldData;


            return dd;
        }
        private OperationalInfo GetOperationalInfoFromMenu(NEMenuOI menuOI)
        {
            var oi = new OperationalInfo();
            oi.ExpiryDate = menuOI.ExpiryDate.FieldData;
            oi.FSRef = menuOI.FSRef.FieldData;
            oi.Power = menuOI.Power.FieldData;
            oi.StartDate = menuOI.StartDate.FieldData;
            oi.Type = menuOI.Type.FieldData;


            return oi;
        }

        private WantedMissing GetWantedMissingFromMenu(NEMenuWM2 menuWM)
        {
            var wm = new WantedMissing();
            wm.CasePapers = menuWM.CasePapers.FieldData;
            wm.Class = menuWM.Class.FieldData;
            //wm.Date = menuWM.ConditionList.FieldData;
            wm.Count = menuWM.Count;
            wm.DateOn = menuWM.DateOn.FieldData;
            wm.DateToDateTo = menuWM.DateTo.FieldData;
            wm.EndDate = menuWM.EndDate.FieldData;
            wm.For = menuWM.For.FieldData;
            wm.FSRef = menuWM.FSRef.FieldData;
            wm.IssuedAt = menuWM.IssuedAt.FieldData;
            wm.Line13 = menuWM.Line13.FieldData;
            wm.Line14 = menuWM.Line14.FieldData;
            wm.Line15 = menuWM.Line15.FieldData;
            wm.Line16 = menuWM.Line16.FieldData;
            wm.Line17 = menuWM.Line17.FieldData;
            wm.Line18 = menuWM.Line18.FieldData;
            wm.Line19 = menuWM.Line19.FieldData;
            wm.Line20 = menuWM.Line20.FieldData;
            wm.Line21 = menuWM.Line21.FieldData;
            wm.Line22 = menuWM.Line22.FieldData;
            wm.Location = menuWM.Location.FieldData;
            wm.PageRef = menuWM.PageRef.FieldData;
            wm.Power = menuWM.Power.FieldData;
            wm.Reported = menuWM.Reported.FieldData;
            wm.SACountry = menuWM.SACountry.FieldData;
            wm.SACreated = menuWM.SACreated.FieldData;
            wm.SADOB = menuWM.SADOB.FieldData;
            wm.SAEAWText = menuWM.SAEAWText.FieldData;
            wm.SAExpires = menuWM.SAExpires.FieldData;
            wm.SAIdentMark1 = menuWM.SAIdentMark1.FieldData;
            wm.SAIdentMark2 = menuWM.SAIdentMark2.FieldData;
            wm.SAIdStatus = menuWM.SAIdStatus.FieldData;
            wm.SALastUpdated = menuWM.SALastUpdated.FieldData;
            wm.SALinked = menuWM.SALinked;
            wm.SAName = menuWM.SAName.FieldData;
            wm.SANationality1 = menuWM.SANationality1.FieldData;
            wm.SANationality2 = menuWM.SANationality2.FieldData;
            wm.SANationality3 = menuWM.SANationality3.FieldData;
            wm.SAOffenceCategory = menuWM.SAOffenceCategory.FieldData;
            wm.SARiskStatus = menuWM.SARiskStatus.FieldData;
            wm.SAStatus = menuWM.SAStatus.FieldData;
            wm.SAText1 = menuWM.SAText1.FieldData;
            wm.SAText2 = menuWM.SAText2.FieldData;
            wm.SAText3 = menuWM.SAText3.FieldData;
            wm.SAText4 = menuWM.SAText4.FieldData;
            wm.SAText5 = menuWM.SAText5.FieldData;
            wm.SAText6 = menuWM.SAText6.FieldData;
            wm.SAText7 = menuWM.SAText7.FieldData;
            wm.SAText8 = menuWM.SAText8.FieldData;
            wm.SAText9 = menuWM.SAText9.FieldData;
            wm.SAText10 = menuWM.SAText10.FieldData;
            wm.SAText11 = menuWM.SAText11.FieldData;
            wm.SAText12 = menuWM.SAText12.FieldData;
            wm.SATRAliasCount = menuWM.SATRAliasCount;
            //wm.Owner = menuWM.SATRAliasList.FieldData;
            wm.SATRBirthplace1 = menuWM.SATRBirthplace1.FieldData;
            wm.SATRBirthplace2 = menuWM.SATRBirthplace2.FieldData;
            wm.SATRBirthplace3 = menuWM.SATRBirthplace3.FieldData;
            wm.SATRBirthplace5 = menuWM.SATRBirthplace5.FieldData;
            wm.SATRBirthplace4 = menuWM.SATRBirthplace4.FieldData;
            wm.SATRFilename = menuWM.SATRFilename.FieldData;
            wm.SAType = menuWM.SAType.FieldData;
            wm.SISId = menuWM.SISId.FieldData;
            wm.Text1 = menuWM.Text1.FieldData;
            wm.Text2 = menuWM.Text2.FieldData;
            wm.Text3 = menuWM.Text3.FieldData;
            wm.WarrantBailIssuedAt = menuWM.WarrantBailIssuedAt.FieldData;
            wm.WarrantIssuedAt = menuWM.WarrantIssuedAt.FieldData;
            wm.Weed = menuWM.Weed.FieldData;


            return wm;
        }

        private BailCondition GetBailConditionsFromMenu(NEMenuBC menuBC)
        {
            var bc = new BailCondition();
            bc.Alert = menuBC.Alert.FieldData;
            bc.Arrest = menuBC.Arrest.FieldData;
            bc.BailAddress = menuBC.BailAddress.FieldData;
            //bc.Owner = menuBC.ConditionList.FieldData;
            bc.CountValue = menuBC.Count.FieldData;
            bc.NextValue = menuBC.Next.FieldData;
            bc.Remand = menuBC.Remand.FieldData;

            foreach (var c in menuBC.ConditionList)
            {
                var con = new BailConditionCondition();
                con.Text1 = c.Text1.FieldData;
                con.Text2 = c.Text2.FieldData;
                con.Text3 = c.Text3.FieldData;
                con.Text4 = c.Text4.FieldData;

                bc.Conditions.Add(con);
            }


            return bc;
        }
        private Address GetAddressFromMenu(NEMenuAD menuAD)
        {
            var ad = new Address();
            ad.Address1 = menuAD.Address1.FieldData;
            ad.Address2 = menuAD.Address2.FieldData;
            ad.Address3 = menuAD.Address3.FieldData;
            ad.Description = menuAD.Description.FieldData;
            ad.Owner = menuAD.Owner.FieldData;
            ad.Updated = menuAD.Updated.FieldData;
            

            return ad;
        }
        private DisposalSummary GetDisposalSummaryFromMenu(NEMenuDS menuDS)
        {
            var ds = new DisposalSummary();
            ds.DSExtraTextCount = menuDS.DSExtraTextCount;
            ds.DSExtraTextList = menuDS.DSExtraTextList;
            ds.DSOffenceCount = menuDS.DSOffenceCount;
            //ds.Owner = menuDS.DSOffenceList;
            ds.FirstValue = menuDS.First.FieldData;
            ds.LastValue = menuDS.Last.FieldData;
            ds.TotalCautions = menuDS.TotalCautions.FieldData;
            ds.TotalConvictions = menuDS.TotalConvictions.FieldData;
            ds.TotalDeport = menuDS.TotalDeport.FieldData;
            ds.TotalNFA = menuDS.TotalNFA.FieldData;
            ds.TotalNINonCourtDisposals = menuDS.TotalNINonCourtDisposals.FieldData;
            ds.TotalNonConvictions = menuDS.TotalNonConvictions.FieldData;
            ds.TotalNotGuilty = menuDS.TotalNotGuilty.FieldData;
            ds.TotalPenaltyNotices = menuDS.TotalPenaltyNotices.FieldData;
            ds.TotalRefer = menuDS.TotalRefer.FieldData;
            ds.TotalReprimands = menuDS.TotalReprimands.FieldData;
            ds.TotalUnobtainable = menuDS.TotalUnobtainable.FieldData;
            ds.TotalWarnings = menuDS.TotalWarnings.FieldData;

            foreach (var o in menuDS.DSOffenceList)
            {
                var offence = new Offence();
                offence.OffenceCount = o.OffenceCount.FieldData;
                offence.OffenceDescription = o.OffenceDescription.FieldData;
                offence.OffencePeriod = o.OffencePeriod.FieldData;
                ds.Offences.Add(offence);
            }

            return ds;
        }
        private Description GetDescriptionFromMenu(NEMenuDE menuDE)
        {
            var de = new Description();
            de.Accent = menuDE.Accent.FieldData;
            de.BirthDay = menuDE.BirthDay.FieldData;
            de.BirthMonth = menuDE.BirthMonth.FieldData;
            de.BirthYear = menuDE.BirthYear.FieldData;
            de.Build = menuDE.Build.FieldData;
            de.POB = menuDE.POB.FieldData;
            de.Colour = menuDE.Colour.FieldData;
            de.DriverNumber1 = menuDE.DriverNumber1.FieldData;
            de.DriverNumber2 = menuDE.DriverNumber2.FieldData;
            de.DriverNumber3 = menuDE.DriverNumber3.FieldData;
            de.EACode = menuDE.EACode.FieldData;
            de.EyeColour1 = menuDE.EyeColour1.FieldData;
            de.EyeColour2EyeColour2 = menuDE.EyeColour2.FieldData;
            de.FacialHair1Colour = menuDE.FacialHair1Colour.FieldData;
            de.FacialHair1Colour2 = menuDE.FacialHair1Colour2.FieldData;
            de.FacialHair1DG = menuDE.FacialHair1DG.FieldData;
            de.FacialHair1Features = menuDE.FacialHair1Features.FieldData;
            de.FacialHair1Type = menuDE.FacialHair1Type.FieldData;
            de.FacialHair2Colour = menuDE.FacialHair2Colour.FieldData;
            de.FacialHair2Colour2 = menuDE.FacialHair2Colour2.FieldData;
            de.FacialHair2DG = menuDE.FacialHair2DG.FieldData;
            de.FacialHair2Features = menuDE.FacialHair2Features.FieldData;
            de.FacialHair2Type = menuDE.FacialHair2Type.FieldData;
            de.FacialHair3Colour = menuDE.FacialHair3Colour.FieldData;
            de.FacialHair3Colour2 = menuDE.FacialHair3Colour2.FieldData;
            de.FacialHair3DG = menuDE.FacialHair3DG.FieldData;
            de.FacialHair3Features = menuDE.FacialHair3Features.FieldData;
            de.FacialHair3Type = menuDE.FacialHair3Type.FieldData;
            de.FileName = menuDE.FileName.FieldData;
            de.Glasses = menuDE.Glasses.FieldData;
            de.HairColour = menuDE.HairColour.FieldData;
            de.HairColour2 = menuDE.HairColour2.FieldData;
            de.HairDG = menuDE.HairDG.FieldData;
            de.HairFeatures1 = menuDE.HairFeatures1.FieldData;
            de.HairFeatures2 = menuDE.HairFeatures2.FieldData;
            de.HairType = menuDE.HairType.FieldData;
            de.Handedness = menuDE.Handedness.FieldData;
            de.Height = menuDE.Height.FieldData;
            de.MSA1 = menuDE.MSA1.FieldData;
            de.MSA2 = menuDE.MSA2.FieldData;
            de.MSA3 = menuDE.MSA3.FieldData;
            de.MSA4 = menuDE.MSA4.FieldData;
            de.Nationality1 = menuDE.Nationality1.FieldData;
            de.Nationality2 = menuDE.Nationality2.FieldData;
            de.Nationality3 = menuDE.Nationality3.FieldData;
            de.POB = menuDE.POB.FieldData;
            de.Sex = menuDE.Sex.FieldData;
            de.ShoeSize = menuDE.ShoeSize.FieldData;

            return de;
        }

        private WarningSignal GetWarningSignalFromMenu(NEMenuWS menuWS)
        {
            var ws = new WarningSignal();
            ws.Date = menuWS.Date.FieldData;
            ws.Updated = menuWS.Updated.FieldData;
            ws.Warning = menuWS.Warning.FieldData;
            ws.Text = menuWS.Text.FieldData;
            ws.FSRef = menuWS.FSRef.FieldData;



            return ws;
        }

        private AliasName GetAliasFromMenu(NEMenuAL menuAL)
        {
            var al = new AliasName();
            al.Date = menuAL.Date.FieldData;
            al.Name = menuAL.Name.FieldData;
            al.Owner = menuAL.Owner.FieldData;

            return al;
        }
        private AliasDateOfBirth GetAliasDOBFromMenu(NEMenuAB menuAB)
        {
            var al = new AliasDateOfBirth();
            al.Date = menuAB.Date.FieldData;
            al.AliasDOB = menuAB.AliasDOB.FieldData;
            al.Owner = menuAB.Owner.FieldData;

            return al;
        }
        private InformationMarker GetInformationMarkerFromMenu(NEMenuIM menuIM)
        {
            var im = new InformationMarker();
            im.Date = menuIM.Date.FieldData;
            im.FSRef = menuIM.FSRef.FieldData;
            im.Marker = menuIM.Marker.FieldData;
            im.Text = menuIM.Text.FieldData;
            im.Updated = menuIM.Updated.FieldData;
            

            return im;
        }

        [HttpGet("{surname}")]
        [HttpGet("{surname}/{forename1}")]
        [HttpGet("{surname}/{forename1}/{forename2}")]
        public ActionResult<PersonWrapper> Get(string surname, string forename1, string forename2,string dateofbirth,string gender,string ethnicity,string reason,string isnominal,string isdrivinglicense,string postcode)
        {
            var allPersons = new PersonWrapper();
            if (isnominal=="true")
            { 
                var nomResults = NominalSearch(surname, forename1, forename2, dateofbirth, gender, ethnicity, reason);
                var persons = nomResults.Value;
                allPersons.Records.AddRange(persons.Records);
            }

            if (isdrivinglicense == "true")
            {
                var dlResults = GetByDL(surname, forename1,forename2,dateofbirth,gender,ethnicity,reason,postcode);
                var persons = dlResults.Value;

                var toRemove = new List<PersonQueryResult>();

                foreach (var driver in persons.Records)
                {
                    var match = allPersons.Records.SingleOrDefault(x => x.DriverNumber.ToUpper() == driver.DriverNumber.ToUpper());
                    if (match != null)
                    {
                        match.ResultFrom = "PNC/DL";
                        toRemove.Add(driver);
                    }

                }
                foreach (var remove in toRemove)
                {
                    persons.Records.Remove(remove);

                }
                allPersons.Records.AddRange(persons.Records);

            }

            return allPersons;
            
        }

        private ActionResult<PersonWrapper> NominalSearch(string surname, string forename1, string forename2, string dateofbirth, string gender, string ethnicity, string reason)
        {

            var wrap = new PersonWrapper();

            var p1 = new PersonQueryResult();
            p1.Address1 = "DORMER COTTAGE";
            p1.Address2 = "TOOT BALDON";
            p1.Address3 = "OXFORD";
            p1.DateOfBirth = "10031945";
            p1.DriverNumber = "SMITH703101M99SV";
            p1.Ethnicity = "W";
            p1.Forenames = "FRED";
            p1.Gender = "M";
            p1.Name = "FRED SMITH";
            p1.PlaceOfBirth = "ABINGDON";
            p1.PNCId = "1234/D";
            p1.Postcode = "OX44 9HG";
            p1.ResultFrom = "PNC";
            p1.Surname = "SMITH";

            wrap.Records.Add(p1);

            var p2 = new PersonQueryResult();
            p2.Address1 = "11 CLARENCE ROAD";
            p2.Address2 = "NEWPORT PAGNELL";
            p2.Address3 = "MILTON KEYNES";
            p2.DateOfBirth = "10/08/1245";
            p2.DriverNumber = "ADAMS123456M99SV";
            p2.Ethnicity = "W";
            p2.Forenames = "CHARLES";
            p2.Gender = "M";
            p2.Name = "CHARLES ADAMS";
            p2.PlaceOfBirth = "DEADWOOD";
            p2.PNCId = "ADA/123";
            p2.Postcode = "MN12 3DS";
            p2.ResultFrom = "PNC";
            p2.Surname = "ADAMS";

            wrap.Records.Add(p2);

            return wrap;


           
        }

        private static PersonQueryResult GetPersonQueryResult(NEPossible p, PNCScreen next)
        {
            var person = new PersonQueryResult();
            person.Name = p.Name.FieldData;
            //if (!string.IsNullOrEmpty(p.DOB.FieldData))
            //{
            person.DateOfBirth = p.DOB.FieldData;
            //}
            person.Forenames = next.NEResult.ForeNames.FieldData;
            person.Surname = next.NEResult.Surname.FieldData;
            person.PNCId = p.PNCID.FieldData;
            person.Gender = next.NEResult.Sex.FieldData;
            person.Ethnicity = next.NEResult.Colour.FieldData;
            person.PlaceOfBirth = p.Birthplace.FieldData;
            person.Address1 = next.NEResult.FullAddress1.FieldData;
            person.Address2 = next.NEResult.FullAddress2.FieldData;
            person.Address3 = next.NEResult.FullAddress3.FieldData;
            person.DriverNumber = next.NEResult.DriverNo.FieldData.Replace("/", "");
            person.ResultFrom = "PNC";
            return person;
        }

        
    }
}
#endif