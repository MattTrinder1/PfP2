#if MOCK
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NDIApiWrapper.Models;
using NDIXML;
 

namespace NDIApiWrapper.Controllers
{
    [Route("api/vehicle")]
    [ApiController]
    public class MockVehicleController : ControllerBase
    {
        IConfiguration config;
        ILogger log;


        //getbypncid/{pncid}"
        [HttpGet("getbyvrn/{vrn}")]
        public ActionResult<Vehicle> GetByVRN(string vrn, string reason, bool includeInsurance)
        {

            var v = new Vehicle();
            v.VrnReg = vrn;
            v.Make = "Ford(UK)";
            v.Model = "Mondeo";
            v.Colour = "Silver";
            v.Style = "Sedan";
            v.Registered = "1992";
            v.cc = "2000";
            v.ChassisNo = "1234dfdsfds";
            v.EngineNo = "3434";
            v.Stolen = "no";
            v.VELNumber = "123";
            v.VELDate = "1123232";
            v.VROLit = "";
            v.KeeperNotify = "";
            v.PreviousVRNReg = "";
            v.MOTExpiry = "1003";
            v.InsuranceText = "asdf"; ;
            v.InsuranceText2 = ""; ;
            v.InsuranceFlag = ""; ;
            v.HazardsFlag = ""; ;
            v.TagNumber = "";
            v.NoOfReports = 2;

            var i = new InsuranceDetails();
            v.Insurance.Add(i);
            i.Address1 = "11 CLARENCE ROAD,STONY STRATFORD";
            i.Address2 = "MILTON KEYNES,MK11 1JE";
            i.AllowedOthers = "Yes";
            i.ClassLine1 = "Public";
            i.InsuranceHeld = "FULLY COMPREHENSIVE";
            i.MakeModel = "FORD MONDEO";
            i.PermittedDrivers1 = "Insured and Spouse";


            var r = new Report();
            r.ReportNum = "1";
            r.ReportType = "THEFT";
            r.Text1 = "STOLEN";
            v.Reports = new List<Report>() { r }.ToArray();
            v.NoOfReports = 1;

            var k = new Keeper();
            k.ad1 = "100 WOLVERTON ROAD";
            k.RegKeeper = "MATT TRINDER";
            v.Keepers.Add(k);

            var m = new Marker();
            m.Text = "Marker 1";
            v.Markers.Add(m);

            return v;

        }

        private static void GetNamedDriver(InsuranceDetails insurance, Field namedDriverExcl)
        {
            if (!namedDriverExcl.FieldData.EndsWith("+"))
            {
                insurance.NamedDrivers.Add(new NamedDriver() { Number = Convert.ToInt32(namedDriverExcl.FieldData.Substring(0,1)), Name = namedDriverExcl.FieldData.Substring(2, 50).Trim(), DOB = namedDriverExcl.FieldData.Substring(namedDriverExcl.FieldData.Length - 10, 10) });
            }
            
        }


        // GET api/values/5
        [HttpGet("{vrn}")]
        public ActionResult<VehicleWrapper> Get(string VRN, string reason)
        {
            using (var sessionWrapper = new SessionWrapper(config.GetValue<string>("Url"), config.GetValue<string>("User"), config.GetValue<string>("Session")))
            {
                var sw = new Stopwatch();
                sw.Start();

                
                //run quer
                var screen = sessionWrapper.logonResponse.TCODEScreen;
                screen.Data.FieldData = VRN;
                screen.Originator.FieldData = config.GetValue<string>("Origin");
                screen.ReasonCode.FieldData = reason;

                var s = new PNCScreen();
                s.TCODEScreen = screen;
                s.Session = sessionWrapper.connectResponse.Session;

                var vehicleResponse = sessionWrapper.client.HashVE(s);
                log.LogDebug($"PNC HashVE : {sw.ElapsedMilliseconds.ToString()}");
                sw.Reset(); sw.Start();


                var vehicles = new List<Vehicle>();

                if (vehicleResponse.Rawscreen.Line3.Trim().Contains("NO TRACE OF VRM"))
                {
                    return new VehicleWrapper();
                }

                while (vehicleResponse.Control.Reason.ToLower() != "no more records")
                {




                    int reportIndex = 0;
                    int reportCount = 0;
                    if (!vehicleResponse.Rawscreen.Line17.Contains("NO REPORTS PRESENT"))
                    {
                        reportIndex = Convert.ToInt32(vehicleResponse.Rawscreen.Line17.Substring(44, 1));
                        reportCount = Convert.ToInt32(vehicleResponse.Rawscreen.Line17.Substring(49, 1));
                    }

                    Vehicle v = GetVehicleFromResponse(vehicleResponse);

                    var reports = new List<Report>();
                    v.Reports = reports.ToArray();
                    v.NoOfReports = reportCount;
                    vehicles.Add(v);
                    vehicleResponse = sessionWrapper.client.NextVERecord(sessionWrapper.connectResponse.Session.SessionInfo);
                    log.LogDebug($"PNC NextVE : {sw.ElapsedMilliseconds.ToString()}");
                    sw.Reset(); sw.Start();

                }


                return new VehicleWrapper() { Records = vehicles };
            }
        }

        private static Vehicle GetVehicleFromResponse(PNCScreen vehicleResponse)
        {
            var v = new Vehicle();
            v.VrnReg = vehicleResponse.Rawscreen.Line5.Substring(1, 11).Trim();
            v.Make = vehicleResponse.Rawscreen.Line6.Substring(1, 16).Trim();
            v.Model = vehicleResponse.Rawscreen.Line7.Substring(1, 25).Trim();
            v.Colour = vehicleResponse.Rawscreen.Line8.Substring(1, 17).Trim();
            v.Style = vehicleResponse.Rawscreen.Line9.Substring(1, 20).Trim();
            v.Registered = vehicleResponse.Rawscreen.Line6.Substring(36, 5).Trim();
            v.cc = vehicleResponse.Rawscreen.Line7.Substring(36, 5).Trim();
            v.ChassisNo = vehicleResponse.Rawscreen.Line10.Substring(6, 20).Trim();
            v.EngineNo = vehicleResponse.Rawscreen.Line11.Substring(6, 20).Trim();
            var keeper = new Keeper();
            keeper.RegKeeper = vehicleResponse.Rawscreen.Line5.Substring(42, 37).Trim();
            keeper.ad1 = vehicleResponse.Rawscreen.Line6.Substring(42, 31).Trim();
            keeper.ad2 = vehicleResponse.Rawscreen.Line7.Substring(42, 36).Trim();
            keeper.ad3 = vehicleResponse.Rawscreen.Line8.Substring(42, 36).Trim();
            keeper.ad4 = vehicleResponse.Rawscreen.Line9.Substring(42, 36).Trim();
            keeper.ad5 = vehicleResponse.Rawscreen.Line10.Substring(42, 36).Trim();
            keeper.ad6 = vehicleResponse.Rawscreen.Line11.Substring(42, 36).Trim();
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line12))
            {
                keeper.KeeperSince = vehicleResponse.Rawscreen.Line12.Substring(55, 8).Trim();
            }
            v.Keepers.Add(keeper);
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line14))
            {
                v.Stolen = vehicleResponse.Rawscreen.Line14.Substring(1, 6).Trim();
                if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line14.Substring(8, 66).Trim()))
                {
                    v.Markers.Add(new Marker() { Text = vehicleResponse.Rawscreen.Line14.Substring(8, 66).Trim() });
                }


            }
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line15))
            {
                if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line15.Substring(1, 78).Trim()))
                {
                    v.Markers.Add(new Marker() { Text = vehicleResponse.Rawscreen.Line15.Substring(1, 78).Trim() });
                }
            }
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line16))
            {
                if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line16.Substring(1, 78).Trim()))
                {
                    v.Markers.Add(new Marker() { Text = vehicleResponse.Rawscreen.Line16.Substring(1, 78).Trim() });
                }
            }
            //v.HazardsFlag
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line9))
            {
                v.VELNumber = vehicleResponse.Rawscreen.Line9.Substring(32, 13).Trim();
            }
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line10))
            {
                v.VELDate = vehicleResponse.Rawscreen.Line10.Substring(31, 8).Trim();
            }
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line12))
            {
                v.VROLit = vehicleResponse.Rawscreen.Line12.Substring(1, 37).Trim();
            }
            v.KeeperNotify = vehicleResponse.Rawscreen.Line13.Substring(42, 37).Trim();
            v.PreviousVRNReg = vehicleResponse.Rawscreen.Line5.Substring(28, 11).Trim();
            v.MOTExpiry = vehicleResponse.Rawscreen.Line13.Substring(30, 12).Trim();
            v.InsuranceText = vehicleResponse.Rawscreen.Line13.Substring(1, 16).Trim();
            if (!string.IsNullOrEmpty(vehicleResponse.Rawscreen.Line4))
            {
                v.InsuranceText2 = vehicleResponse.Rawscreen.Line4.Substring(34, 40).Trim();
            }
            v.HazardsFlag = vehicleResponse.Rawscreen.Line8.Substring(31, 6).Trim();
            v.TagNumber = vehicleResponse.Rawscreen.Line9.Substring(28, 20).Trim();
            return v;
        }

        public Report GetReport(Rawscreen raw)
        {
            var r = new Report();
            r.ReportType = raw.Line18.Substring(1, 15).Trim();
            r.ReportFSOwner= raw.Line18.Substring(39, 4).Trim();
            r.ReportFSCreator = raw.Line18.Substring(54, 4).Trim();
            r.ReportNum = raw.Line18.Substring(65, 14).Trim();
            r.ReportEarlyDate = raw.Line19.Substring(11, 8).Trim();
            r.ReportLateDate = raw.Line19.Substring(21, 8).Trim();
            r.ReportCreatedDate= raw.Line19.Substring(62, 8).Trim();
            r.ReportCreatedTime = raw.Line19.Substring(74, 5).Trim();
            if (!string.IsNullOrEmpty(raw.Line20))
            {
                r.Text1 = raw.Line20.Substring(11, 66).Trim();
            }
            if (!string.IsNullOrEmpty(raw.Line21))
            {
                r.Text2 = raw.Line21.Substring(11, 66).Trim();
            }
            if (!string.IsNullOrEmpty(raw.Line22))
            {
                r.Text3 = raw.Line22.Substring(11, 66).Trim();
            }
            //r.ReportStatus = raw.Line18.Substring(17, 11).Trim();

            return r;
        }

    }

    
}
#endif