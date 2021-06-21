using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Models.Base
{
    public class DVBase
    {

        [JsonPropertyName("ownerid@odata.bind")]
        [RelatedEntityName("systemuser")]
        public EntityReference ownerid { get; set; }

        public Guid? _ownerid_value { get; init; }

        //   public string IncidentNumber { get; set; }
        //  public DateTime? IncidentDate { get; set; }
    }
}
