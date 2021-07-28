using API.Models.Base;
using API.Models.IYC;
using Common.Models.Business;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace API.Models.PNB
{
    [DataContract(Name = "cp_pocketnotebook")]
    public class DVPocketNotebook : DVBase
    {
        public DVPocketNotebook()
        {            
        }

        public Guid? cp_pocketnotebookid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_notes { get; set; }
       
        public DateTime? cp_notedateandtime{ get; set; }


        [RelatedEntityName("cp_incident")]
        [JsonPropertyName("cp_IncidentNo@odata.bind")]
        public EntityReference cp_incidentno { get; set; }
        public Guid? _cp_incidentno_value { get; set; }


        public DateTime? cp_signaturedateandtime { get; set; }
        public string cp_signatoryname { get; set; }
    }


    [DataContract(Name = "cp_pocketnotebook")]
    public class DVPocketNotebookImages : DVBase
    {
        public DVPocketNotebookImages()
        {
        }

        public Guid? cp_pocketnotebookid { get { return base.Id; } set { base.Id = value.Value; } }

        [DVImage()]
        public string cp_sketch { get; set; }

        [DVImage()]
        public string cp_signature { get; set; }
    }
}
