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

        public DVBase()
        {
            Id = Guid.NewGuid();
        }


        //[JsonPropertyName("ownerid@odata.bind")]
        //[RelatedEntityName("systemuser")]
        //public string ownerid { get; set; }

        [JsonIgnore]
        public Guid Id { get; set; }

        //public Guid? _ownerid_value { get; init; }

        //   public string IncidentNumber { get; set; }
        //  public DateTime? IncidentDate { get; set; }
    }
}
