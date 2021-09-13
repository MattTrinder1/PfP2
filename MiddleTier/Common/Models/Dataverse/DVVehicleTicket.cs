using API.Models.Base;
using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    [DataContract(Name = "cp_vehicleticket")]

    public class DVVehicleTicket : DVBase
    {
        public DateTime? cp_offencedatetime { get; set; }

        [JsonPropertyName("cp_issuedto@odata.bind")]
        [RelatedEntityName("cp_lookupvalue")]
        public EntityRef cp_issuedto { get; set; }

        public Guid? _cp_issuedto_value { get; init; }

        public string cp_policevehiclenumber { get; set; }
    }
}
