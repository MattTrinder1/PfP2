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
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        IConfiguration config;
        ILogger log;
        public PersonController(IConfiguration configuration,ILogger<PersonController> logger )
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

            using (var sessionWrapper = new SessionWrapper(config.GetValue<string>("Url"), config.GetValue<string>("User"), config.GetValue<string>("Session")))
            { 

                sw.Start();

                if (sessionWrapper.logonResponse.Control.Result != "SUCCESS")
                {
                    return NotFound();
                }

                var screen = sessionWrapper.logonResponse.TCODEScreen;
                screen.Data.FieldData = driverNumber;
                screen.Originator.FieldData = config.GetValue<string>("Origin");
                screen.ReasonCode.FieldData = reason;

                var s = new PNCScreen();
                s.TCODEScreen = screen;
                s.Session = sessionWrapper.connectResponse.Session;

                var wrap = new PersonWrapper();


                var personResponse = sessionWrapper.client.HashDL(s);

                if (personResponse.DLResult == null)
                {
                    return null;
                }
                else
                {
                    //should be only one
                    var pers = personResponse.DLResult;
                    var person = new Person();
                    person.FirstName = pers.Name.FieldData;
                    person.LastName = pers.Name.FieldData;
                    person.Title = pers.Title.FieldData;
                    person.DateOfBirth = pers.Birth.FieldData;
                    person.DriverNumber = pers.DriverNo.FieldData;
                    person.Gender = pers.Sex.FieldData;
                    person.LicenceStatus = pers.LicenceStatus.FieldData;
                    person.Disqualification = pers.Disqualification.FieldData;
                    person.Address1 = pers.FullAddress1.FieldData;
                    person.Address2 = pers.FullAddress2.FieldData;
                    person.Address3 = pers.FullAddress3.FieldData;
                    if (!string.IsNullOrEmpty(pers.FullAddress4.FieldData))
                    {
                        person.Address3 += "," + pers.FullAddress4.FieldData;
                    }
                    if (!string.IsNullOrEmpty(pers.FullAddress5.FieldData))
                    {
                        person.Address3 += "," + pers.FullAddress5.FieldData;
                    }


                    person.PostCode = pers.PostCode.FieldData;
                    person.LicenceType = pers.LicenceType.FieldData;
                    person.LicenceIssue = pers.LicenceIssue.FieldData;
                    person.CounterPartIssue = pers.CounterpartIssue.FieldData;
                    person.CommencementDate = pers.CommencementDate.FieldData;
                    person.ExpiryDate = pers.ExpiryDate.FieldData;
                    person.Birthplace = pers.Birthplace.FieldData;


                    PNCScreen menuResult;
                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "ED");
                    if (menuResult.DLMenuED != null)
                    {
                        person.Endorsements.Add(GetEndorsementFromMenu(menuResult));
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        while (menuResult.DLMenuED != null)
                        {
                            person.Endorsements.Add(GetEndorsementFromMenu(menuResult));
                            menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        }
                    }


                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "ES");
                    person.DrivingSummary = GetDrivingSummaryFromMenu(menuResult);

                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "FE");
                    person.FullEntitlement.Add(GetFullEntitlementFromMenu(menuResult));
                    menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                    while (menuResult.DLMenuFE != null)
                    {
                        person.FullEntitlement.Add(GetFullEntitlementFromMenu(menuResult));
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                    }


                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "PE");
                    person.ProvisionalEntitlement.Add(GetProvisionalEntitlementFromMenu(menuResult));
                    menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                    while (menuResult.DLMenuPE != null)
                    {
                        person.ProvisionalEntitlement.Add(GetProvisionalEntitlementFromMenu(menuResult));
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                    }

                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "UT");
                    if (menuResult.DLMenuUT != null)
                    {
                        person.Unclaimeds.Add(GetUnclaimedFromMenu(menuResult));
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        while (menuResult.DLMenuUT != null)
                        {
                            person.Unclaimeds.Add(GetUnclaimedFromMenu(menuResult));
                            menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        }
                    }

                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "SM");
                    if (menuResult.DLMenuSM != null)
                    {
                        person.StopsMarkers.Add(GetStopMarkerFromMenu(menuResult));
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        while (menuResult.DLMenuSM != null)
                        {
                            person.StopsMarkers.Add(GetStopMarkerFromMenu(menuResult));
                            menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        }
                    }

                    //docuemnt trail
                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "DT");
                    var doc = GetDocumentFromMenu(menuResult);
                    person.DocumentTrail.Add(doc);
                    menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                    while (menuResult.DLMenuDT != null)
                    {
                        doc = GetDocumentFromMenu(menuResult);
                        person.DocumentTrail.Add(doc);
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);

                    }


                    menuResult = sessionWrapper.client.LicenceMenuParsed(s.Session.SessionInfo, "XR");
                    if (menuResult.DLMenuXR != null)
                    {
                        person.CrossRefs.Add(GetCrossRefFromMenu(menuResult));
                        menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        while (menuResult.DLMenuXR != null)
                        {
                            person.CrossRefs.Add(GetCrossRefFromMenu(menuResult));
                            menuResult = sessionWrapper.client.LicenceMenuParsedMore(s.Session.SessionInfo);
                        }
                    }


                    log.LogDebug($"PNC Query : {sw.ElapsedMilliseconds.ToString()}");
                    sw.Reset(); sw.Start();

                    return person;
                }
            }
        }

        private static CrossRef GetCrossRefFromMenu(PNCScreen menuResult)
        {
            CrossRef doc = new CrossRef();
            doc.Driver = menuResult.DLMenuXR.Driver.FieldData;
            doc.Date = menuResult.DLMenuXR.Date.FieldData;
            doc.Name1 = menuResult.DLMenuXR.Name1.FieldData;
            doc.Name2 = menuResult.DLMenuXR.Name2.FieldData;
            doc.Name3 = menuResult.DLMenuXR.Name3.FieldData;
            doc.Name4 = menuResult.DLMenuXR.Name4.FieldData;
            
            return doc;
        }

            private static Endorsement GetEndorsementFromMenu(PNCScreen menuResult)
        {
            Endorsement doc = new Endorsement();
            doc.AppealCourt = menuResult.DLMenuED.AppealCourt.FieldData;
            doc.AppealDate = menuResult.DLMenuED.AppealDate.FieldData;
            doc.ConvictionCourt = menuResult.DLMenuED.ConvictionCourt.FieldData;
            doc.ConvictionDate = menuResult.DLMenuED.ConvictionDate.FieldData;
            doc.DateDisqRemoved = menuResult.DLMenuED.DateDisqRemoved.FieldData;
            doc.DisqPendAppeal = menuResult.DLMenuED.DisqPendAppeal.FieldData;
            doc.DisqPendSentence= menuResult.DLMenuED.DisqPendSentence.FieldData;
            doc.DisqPeriod = menuResult.DLMenuED.DisqPeriod.FieldData;
            doc.DisqReimposed = menuResult.DLMenuED.DisqReimposed.FieldData;
            doc.DTTPorDPS = menuResult.DLMenuED.DTTPorDPS.FieldData;
            doc.Fine = menuResult.DLMenuED.Fine.FieldData;
            doc.OffenceCode = menuResult.DLMenuED.OffenceCode.FieldData;
            doc.OffenceDate = menuResult.DLMenuED.OffenceDate.FieldData;
            doc.OtherSentence = menuResult.DLMenuED.OtherSentence.FieldData;
            doc.Points = menuResult.DLMenuED.Points.FieldData;
            doc.RehabReduction = menuResult.DLMenuED.RehabReduction.FieldData;
            doc.SentencingCourt = menuResult.DLMenuED.SentencingCourt.FieldData;
            doc.SentencingDate = menuResult.DLMenuED.SentencingDate.FieldData;
            doc.SuspendSentence = menuResult.DLMenuED.SuspendSentence.FieldData;
            return doc;
        }
        private static DrivingSummary GetDrivingSummaryFromMenu(PNCScreen menuResult)
        {
            DrivingSummary doc = new DrivingSummary();
            if (menuResult.DLMenuES != null)
            {
                doc.Disqualified = menuResult.DLMenuES.Disqualified.FieldData;
                doc.DrinkDrug = menuResult.DLMenuES.DrinkDrug.FieldData;
                doc.Other = menuResult.DLMenuES.Other.FieldData;
                doc.Points = menuResult.DLMenuES.Points.FieldData;
            }
            return doc;
        }
        private static Document GetDocumentFromMenu(PNCScreen menuResult)
        {
            Document doc = new Document();
            doc.DocumentRef = menuResult.DLMenuDT.Document.FieldData;
            doc.Date = menuResult.DLMenuDT.Date.FieldData;
            doc.Description1 = menuResult.DLMenuDT.Description1.FieldData;
            doc.Description2 = menuResult.DLMenuDT.Description2.FieldData;
            doc.Description3 = menuResult.DLMenuDT.Description3.FieldData;
            doc.Description4 = menuResult.DLMenuDT.Description4.FieldData;
            return doc;
        }

        private static StopMarker GetStopMarkerFromMenu(PNCScreen menuResult)
        {
            StopMarker doc = new StopMarker();
            doc.Description = menuResult.DLMenuSM.Description.FieldData;

            return doc;
        }
        private static Entitlement GetFullEntitlementFromMenu(PNCScreen menuResult)
        {
            Entitlement doc = new Entitlement();
            doc.Category = menuResult.DLMenuFE.Category.FieldData;
            doc.From = menuResult.DLMenuFE.From.FieldData;
            doc.Until = menuResult.DLMenuFE.Until.FieldData;
            doc.Description1 = menuResult.DLMenuFE.Description1.FieldData;
            doc.Description2 = menuResult.DLMenuFE.Description2.FieldData;
            doc.Description3 = menuResult.DLMenuFE.Description3.FieldData;
            doc.Description4 = menuResult.DLMenuFE.Description4.FieldData;
            doc.Description5 = menuResult.DLMenuFE.Description5.FieldData;
            doc.Restriction1 = menuResult.DLMenuFE.Restriction1.FieldData;
            doc.Restriction2 = menuResult.DLMenuFE.Restriction2.FieldData;
            doc.Restriction3 = menuResult.DLMenuFE.Restriction3.FieldData;
            doc.Restriction4 = menuResult.DLMenuFE.Restriction4.FieldData;
            doc.EU3D = menuResult.DLMenuFE.Description1.FieldData;

            return doc;
        }
        private static Unclaimed GetUnclaimedFromMenu(PNCScreen menuResult)
        {
            Unclaimed doc = new Unclaimed();
            doc.Category = menuResult.DLMenuFE.Category.FieldData;
            doc.Harm = menuResult.DLMenuFE.From.FieldData;
            doc.Date = menuResult.DLMenuFE.Until.FieldData;
            doc.Description1 = menuResult.DLMenuFE.Description1.FieldData;
            doc.Description2 = menuResult.DLMenuFE.Description2.FieldData;
            doc.Description3 = menuResult.DLMenuFE.Description3.FieldData;
            doc.Description4 = menuResult.DLMenuFE.Description4.FieldData;
            doc.Description5 = menuResult.DLMenuFE.Description5.FieldData;
            doc.Restriction1 = menuResult.DLMenuFE.Restriction1.FieldData;
            doc.Restriction2 = menuResult.DLMenuFE.Restriction2.FieldData;
            doc.Restriction3 = menuResult.DLMenuFE.Restriction3.FieldData;
            doc.Restriction4 = menuResult.DLMenuFE.Restriction4.FieldData;
            doc.EU3D = menuResult.DLMenuFE.Description1.FieldData;

            return doc;
        }
        private static Entitlement GetProvisionalEntitlementFromMenu(PNCScreen menuResult)
        {
            Entitlement doc = new Entitlement();
            doc.Category = menuResult.DLMenuPE.Category.FieldData;
            doc.From = menuResult.DLMenuPE.From.FieldData;
            doc.Until = menuResult.DLMenuPE.Until.FieldData;
            doc.Description1 = menuResult.DLMenuPE.Description1.FieldData;
            doc.Description2 = menuResult.DLMenuPE.Description2.FieldData;
            doc.Description3 = menuResult.DLMenuPE.Description3.FieldData;
            doc.Description4 = menuResult.DLMenuPE.Description4.FieldData;
            doc.Description5 = menuResult.DLMenuPE.Description5.FieldData;
            doc.Restriction1 = menuResult.DLMenuPE.Restriction1.FieldData;
            doc.Restriction2 = menuResult.DLMenuPE.Restriction2.FieldData;
            doc.Restriction3 = menuResult.DLMenuPE.Restriction3.FieldData;
            doc.Restriction4 = menuResult.DLMenuPE.Restriction4.FieldData;
            doc.EU3D = menuResult.DLMenuPE.Description1.FieldData;

            return doc;
        }

        [HttpGet("getbydl/{surname}/{forename1}")]
        public ActionResult<PersonWrapper> GetByDL(string surname, string forename1, string forename2, string dateofbirth, string gender, string ethnicity, string reason, string postcode)
        {
            using (var sessionWrapper = new SessionWrapper(config.GetValue<string>("Url"), config.GetValue<string>("User"), config.GetValue<string>("Session")))
            {
                var sw = new Stopwatch();
                sw.Start();

                if (sessionWrapper.logonResponse.Control.Result != "SUCCESS")
                {
                    return NotFound();
                }

                var screen = sessionWrapper.logonResponse.TCODEScreen;
                string query = $"{surname}/{forename1}";
                //screen.Data.FieldData = $"{surname}/{forename1}:03031996:M:N::Y:N";

                query += ":";
                if (!String.IsNullOrEmpty(dateofbirth))
                {
                    query += dateofbirth;
                }
                query += ":";
                if (!String.IsNullOrEmpty(gender))
                {
                    query += gender;
                }
                query += ":N";
                query += ":";
                if (!String.IsNullOrEmpty(postcode))
                {
                    query += postcode;
                }

                query += ":Y:N";
                screen.Data.FieldData = query;
                screen.Originator.FieldData = config.GetValue<string>("Origin");
                screen.ReasonCode.FieldData = reason;

                var s = new PNCScreen();
                s.TCODEScreen = screen;
                s.Session = sessionWrapper.connectResponse.Session;

                var wrap = new PersonWrapper();


                var personResponse = sessionWrapper.client.HashDL(s);

                if (personResponse.DLPossibles != null && personResponse.DLPossibles.DLPossibleList.Length > 0)
                {
                    foreach (var poss in personResponse.DLPossibles.DLPossibleList)
                    {
                        var a = sessionWrapper.client.LicenceSelectPossible(sessionWrapper.connectResponse.Session.SessionInfo, Convert.ToInt32(poss.Identifier.FieldData));
                        var pers = a.DLResult;

                        PersonQueryResult res = GetPersonQueryResultFromDLResult(pers);

                        wrap.Records.Add(res);


                        a = sessionWrapper.client.NextLicenceList(sessionWrapper.connectResponse.Session.SessionInfo);

                    }
                }
                else if (personResponse.DLResult == null)
                {
                    return wrap;
                }
                else
                {
                    //should be only one
                    var pers = personResponse.DLResult;
                    PersonQueryResult res = GetPersonQueryResultFromDLResult(pers);
                    wrap.Records.Add(res);

                }



                log.LogDebug($"PNC Query : {sw.ElapsedMilliseconds.ToString()}");
                sw.Reset(); sw.Start();


                return wrap;
            }
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

            using (var sessionWrapper = new SessionWrapper(config.GetValue<string>("Url"), config.GetValue<string>("User"), config.GetValue<string>("Session")))
            {
                var sw = new Stopwatch();
                sw.Start();


                //logon
               
                var screen = sessionWrapper.logonResponse.TCODEScreen;
                screen.Data.FieldData = pncId.Replace("-", "/");
                screen.Originator.FieldData = config.GetValue<string>("Origin");
                screen.ReasonCode.FieldData = "06";

                var s = new PNCScreen();
                s.TCODEScreen = screen;
                s.Session = sessionWrapper.connectResponse.Session;

                var personResponse = sessionWrapper.client.HashNE(s);
                log.LogDebug($"PNC HashNE Query : {sw.ElapsedMilliseconds.ToString()}");
                sw.Reset(); sw.Start();


                if (personResponse.NEResult == null)
                {
                    return NotFound();
                }

                var p = new Person();
                var res = personResponse.NEResult;
                p.WarningSignalsText = res.WSAlert1.FieldData + " " + res.WSAlert2.FieldData;
                p.FirstName = res.ForeNames.FieldData;
                p.LastName = res.Surname.FieldData;
                p.FileDOB = res.Birth.FieldData;
                p.StatusSum = res.StatusSum.FieldData;

                if (!string.IsNullOrEmpty(res.Birth.FieldData))
                {
                    p.Birthplace = res.Birth.FieldData.Substring(8, res.Birth.FieldData.Length - 8).Trim();
                    p.DateOfBirth = res.Birth.FieldData.Substring(0, 8);
                }

                p.CRONumber = res.CRO.FieldData;
                p.Gender = res.Sex.FieldData;
                p.Ethnicity = res.Colour.FieldData;
                //p.Nationality =res.n
                p.FileName = res.Filename.FieldData;
                p.Address1 = res.FullAddress1.FieldData;
                p.Address2 = res.FullAddress2.FieldData;
                p.Address3 = res.FullAddress3.FieldData;
                p.AddressDate = res.AddrDate.FieldData;
                p.DriverNumber = res.DriverNo.FieldData.Replace("/", "");

                PopulateRelated<NEMenuMS, MarkScar>(sessionWrapper.client, sessionWrapper.connectResponse, res, "MS", p.MarkScars, GetMarkScarFromMenu);
                PopulateRelated<NEMenuIM, InformationMarker>(sessionWrapper.client, sessionWrapper.connectResponse, res, "IM", p.InformationMarkers, GetInformationMarkerFromMenu);
                PopulateRelated<NEMenuWS, WarningSignal>(sessionWrapper.client, sessionWrapper.connectResponse, res, "WS", p.WarningSignals, GetWarningSignalFromMenu);
                PopulateRelated<NEMenuAL, AliasName>(sessionWrapper.client, sessionWrapper.connectResponse, res, "AL", p.AliasNames, GetAliasFromMenu);
                PopulateRelated<NEMenuAB, AliasDateOfBirth>(sessionWrapper.client, sessionWrapper.connectResponse, res, "AB", p.AliasDOBs, GetAliasDOBFromMenu);
                PopulateRelated<NEMenuNK, Nickname>(sessionWrapper.client, sessionWrapper.connectResponse, res, "NK", p.Nicknames, GetNicknameFromMenu);
                PopulateRelated<NEMenuDE, Description>(sessionWrapper.client, sessionWrapper.connectResponse, res, "DE", p.Descriptions, GetDescriptionFromMenu);
                PopulateRelated<NEMenuDS, DisposalSummary>(sessionWrapper.client, sessionWrapper.connectResponse, res, "DS", p.DisposalSummaries, GetDisposalSummaryFromMenu);
                PopulateRelated<NEMenuAD, Address>(sessionWrapper.client, sessionWrapper.connectResponse, res, "AD", p.Addresses, GetAddressFromMenu);
                PopulateRelated<NEMenuBC, BailCondition>(sessionWrapper.client, sessionWrapper.connectResponse, res, "BC", p.BailConditons, GetBailConditionsFromMenu);
                PopulateRelated<NEMenuWM2, WantedMissing>(sessionWrapper.client, sessionWrapper.connectResponse, res, "WM", p.WantedMissings, GetWantedMissingFromMenu);
                PopulateRelated<NEMenuOI, OperationalInfo>(sessionWrapper.client, sessionWrapper.connectResponse, res, "OI", p.OperationalInfos, GetOperationalInfoFromMenu);
                PopulateRelated<NEMenuDD, Disqualified>(sessionWrapper.client, sessionWrapper.connectResponse, res, "DD", p.Disqualifieds, GetDisqualifiedsFromMenu);
                PopulateRelated<NEMenuOD, OtherDetail>(sessionWrapper.client, sessionWrapper.connectResponse, res, "OD", p.OtherDetails, GetOtherDetailsFromMenu);

                log.LogDebug($"PNC Related Queries : {sw.ElapsedMilliseconds.ToString()}");
                sw.Reset(); sw.Start();


                if (!string.IsNullOrEmpty(p.DriverNumber))
                {
                    var driver = GetByDriverNumber(p.DriverNumber, "06");
                    if (driver != null && driver.Value != null)
                    {
                        p.LicenceIssue = driver.Value.LicenceIssue;
                        p.LicenceStatus = driver.Value.LicenceStatus;
                        p.LicenceType = driver.Value.LicenceType;
                        p.Disqualification = driver.Value.Disqualification;
                        p.CounterPartIssue = driver.Value.CounterPartIssue;
                        p.CommencementDate = driver.Value.CommencementDate;
                        p.ExpiryDate = driver.Value.ExpiryDate;
                        p.Endorsements = driver.Value.Endorsements;
                        p.DrivingSummary = driver.Value.DrivingSummary;
                        p.ProvisionalEntitlement = driver.Value.ProvisionalEntitlement;
                        p.FullEntitlement = driver.Value.FullEntitlement;
                        p.Unclaimeds = driver.Value.Unclaimeds;
                        p.StopsMarkers = driver.Value.StopsMarkers;
                        p.DocumentTrail = driver.Value.DocumentTrail;
                        p.CrossRefs = driver.Value.CrossRefs;
                    }

                }

                return p;
            }


            //            return new Person() { FirstName = HttpUtility.UrlDecode(pncId) };
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

            //connect terminal session
            using (var sessionWrapper = new SessionWrapper(config.GetValue<string>("Url"), config.GetValue<string>("User"), config.GetValue<string>("Session")))
            {
                var sw = new Stopwatch();
                sw.Start();


                //run quer
                var screen = sessionWrapper.logonResponse.TCODEScreen;
                var query = surname;
                if (!string.IsNullOrEmpty(forename1))
                {
                    query += "/" + forename1;
                }
                if (!string.IsNullOrEmpty(forename2))
                {
                    query += "/" + forename2;
                }
                query += ":";
                if (!String.IsNullOrEmpty(dateofbirth))
                {
                    query += dateofbirth;
                }
                query += ":";
                if (!String.IsNullOrEmpty(gender))
                {
                    query += gender;
                }
                query += ":";
                if (!String.IsNullOrEmpty(ethnicity))
                {
                    query += ethnicity;
                }
                query += ":";


                screen.Data.FieldData = query;
                screen.Originator.FieldData = config.GetValue<string>("Origin");
                screen.ReasonCode.FieldData = reason;

                var s = new PNCScreen();
                s.TCODEScreen = screen;
                s.Session = sessionWrapper.connectResponse.Session;

                var personResponse = sessionWrapper.client.HashNQ(s);
                log.LogDebug($"PNC HashNQ Query : {sw.ElapsedMilliseconds.ToString()}");
                sw.Reset(); sw.Start();


                //foreach (var menu in personResponse.NEResult.NEMenuAvailableList)
                //{

                //   var m = client.NominalMenuParsed2(connectResponse.Session.SessionInfo, menu.NEMenuShort.FieldData);
                //   while (m.Control.State != "NOMINAL")
                //  {
                //     m = client.NominalMenuParsedMore(connectResponse.Session.SessionInfo);

                //}
                // }
                List<PersonQueryResult> persons = new List<PersonQueryResult>();
                if (personResponse.NEResult != null)
                {
                    var person = new PersonQueryResult();
                    person.Name = personResponse.NEResult.Surname.FieldData + ", " + personResponse.NEResult.ForeNames.FieldData;
                    //if (!string.IsNullOrEmpty(personResponse.NEResult.Birth.FieldData))
                    //{
                    //person.DateOfBirth = Convert.ToDateTime((personResponse.NEResult.Birth.FieldData).Substring(0,8));
                    person.DateOfBirth = personResponse.NEResult.Birth.FieldData.Substring(0, 8);
                    //}
                    person.Gender = personResponse.NEResult.Sex.FieldData;
                    person.Ethnicity = personResponse.NEResult.Colour.FieldData;
                    person.PlaceOfBirth = personResponse.NEResult.Birth.FieldData.Substring(8, personResponse.NEResult.Birth.FieldData.Length - 8).Trim();

                    person.Address1 = personResponse.NEResult.FullAddress1.FieldData;
                    person.Address2 = personResponse.NEResult.FullAddress2.FieldData;
                    person.Address3 = personResponse.NEResult.FullAddress3.FieldData;
                    person.DriverNumber = personResponse.NEResult.DriverNo.FieldData.Replace("/", "");
                    person.Forenames = personResponse.NEResult.ForeNames.FieldData;
                    person.Surname = personResponse.NEResult.Surname.FieldData;
                    person.ResultFrom = "PNC";

                    person.PNCId = personResponse.NEResult.PNCID.FieldData;
                    persons.Add(person);

                }
                else
                {
                    int page = 1;
                    if (personResponse.NEPossibles == null)
                    {
                        return new PersonWrapper();
                    }
                    foreach (var p in personResponse.NEPossibles.NEPossibleList)
                    {
                        var next = sessionWrapper.client.SelectPossible(sessionWrapper.connectResponse.Session.SessionInfo, Convert.ToInt32(p.Identifier.FieldData));
                        log.LogDebug($"PNC SelectPossible : {sw.ElapsedMilliseconds.ToString()}");
                        sw.Reset(); sw.Start();
                        PersonQueryResult person = GetPersonQueryResult(p, next);


                        sessionWrapper.client.NextNominalList2(sessionWrapper.connectResponse.Session.SessionInfo, page);
                        log.LogDebug($"PNC NextNominal : {sw.ElapsedMilliseconds.ToString()}");
                        sw.Reset(); sw.Start();

                        persons.Add(person);
                    }


                    page++;
                    personResponse = sessionWrapper.client.NextNominalList2(sessionWrapper.connectResponse.Session.SessionInfo, page);
                    while (personResponse.NEPossibles != null)
                    {

                        foreach (var p in personResponse.NEPossibles.NEPossibleList)
                        {
                            var next = sessionWrapper.client.SelectPossible(sessionWrapper.connectResponse.Session.SessionInfo, Convert.ToInt32(p.Identifier.FieldData));
                            log.LogDebug($"PNC SelectPossible : {sw.ElapsedMilliseconds.ToString()}");
                            sw.Reset(); sw.Start();
                            PersonQueryResult person = GetPersonQueryResult(p, next);

                            sessionWrapper.client.NextNominalList2(sessionWrapper.connectResponse.Session.SessionInfo, page);
                            log.LogDebug($"PNC NextNominal : {sw.ElapsedMilliseconds.ToString()}");
                            sw.Reset(); sw.Start();

                            persons.Add(person);
                        }

                        page++;
                        personResponse = sessionWrapper.client.NextNominalList2(sessionWrapper.connectResponse.Session.SessionInfo, page);
                    }
                }



                var r = new PersonWrapper();
                r.Records = persons;
                return r;
            }
           
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
