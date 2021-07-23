using API.Models.Base;
using API.Models.IYC;
using Common.Models.Business;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace API.Models.PNB
{
    [DataContract(Name = "cp_incident")]
    public class DVIncident : DVBase
    {
        public DVIncident()
        {            
        }

        public Guid cp_incidentid { get { return base.Id; } set { base.Id = value; } }

        public string cp_incidentnumber { get; set; }
       
        public DateTime? cp_incidentdate { get; set; }
    }
}
