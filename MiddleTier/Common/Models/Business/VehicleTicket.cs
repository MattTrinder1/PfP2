using System;

namespace Common.Models.Business
{
    public class VehicleTicket : EntityBase
    {
        public DateTime? OffenceDateTime { get; set; }
        public LookupValue IssuedTo { get; set; }
        public string PoliceVehicleNumber { get; set; }
    }
}
