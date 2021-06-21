using API.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class VehicleTicket : EntityBase
    {
        public DateTime? OffenceDateTime { get; set; }
        public LookupValue IssuedTo { get; set; }
        public string PoliceVehicleNumber { get; set; }
    }
}
