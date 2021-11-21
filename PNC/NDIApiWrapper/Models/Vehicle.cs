using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDIApiWrapper.Models
{

    public class VehicleWrapper
    {
        public VehicleWrapper()
        {
            Records = new List<Vehicle>();
        }
        public List<Vehicle> Records { get; set; }
    }

    public class Keeper
    {
        public string RegKeeper { get; set; }
        public string ad1 { get; set; }
        public string ad2 { get; set; }
        public string ad3 { get; set; }
        public string ad4 { get; set; }
        public string ad5 { get; set; }
        public string ad6 { get; set; }
        public string KeeperSince { get; set; }
    }

    public class Marker
    {
        public string Text { get; set; }
    }

    public class NamedDriver
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
    }

    public class InsuranceDetails
    {
        public InsuranceDetails()
        {
            NamedDrivers = new List<NamedDriver>();
        }

        public string InsuranceHeld { get; internal set; }
        public string VRM { get; internal set; }
        public string MakeModel { get; internal set; }
        public string Holder { get; internal set; }
        public string Address1 { get; internal set; }
        public string Address2 { get; internal set; }
        public string Insurer { get; internal set; }
        public string ClassLine1 { get; internal set; }
        public string ClassLine2 { get; internal set; }
        public string PolicyNumber { get; internal set; }
        public string AllowedOthers { get; internal set; }
        public string StartDate { get; internal set; }
        public string StartTime { get; internal set; }
        public string ExpDate { get; internal set; }
        public string ExpTime { get; internal set; }
        public string PermittedDrivers1 { get; internal set; }
        public string PermittedDrivers2 { get; internal set; }
        public List<NamedDriver> NamedDrivers { get; set; }

    }

    public class Vehicle
    {

        public Vehicle()
        {
            Keepers = new List<Keeper>();
            Markers = new List<Marker>();
            Insurance = new List<InsuranceDetails>();
        }

        public List<InsuranceDetails> Insurance { get; set; }

        public List<Keeper> Keepers { get; set; }
        public List<Marker> Markers { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
        public string Colour { get; set; }
        public string Style { get; set; }
        public string Registered { get; set; }
        public string cc { get; set; }
        public string VrnReg { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string Stolen { get; set; }
        
        public string VELNumber { get; set; }
        public string VELDate { get; set; }
        public string VROLit { get; set; }

        public string KeeperNotify { get; set; }
        public string PreviousVRNReg { get; set; }
        public string MOTExpiry { get; set; }

        public string InsuranceText { get; set; }
        public string InsuranceText2 { get; set; }
        public string InsuranceFlag { get; set; }
        public string HazardsFlag { get; set; }
        public string TagNumber { get; set; }


        public int NoOfReports { get; set; }
        public Report[] Reports { get; set; }
    }

    public class Report
    {

        public string ReportType{ get; set; }
        public string ReportFSOwner { get; set; }
        public string ReportFSCreator { get; set; }

        public string ReportNum { get; set; }
        public string ReportEarlyDate { get; set; }
        public string ReportLateDate { get; set; }
        public string ReportCreatedDate { get; set; }
        public string ReportCreatedTime { get; set; }

        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string ReportStatus { get; internal set; }
    }
}