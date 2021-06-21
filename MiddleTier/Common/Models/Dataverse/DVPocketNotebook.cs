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

        public Guid? em_pocketnotebookid { get; set; }

        public string em_notes { get; set; }
       
        public DateTime? em_notedate{ get; set; }
        
        public string em_incidentnumber { get; set; }
        
        
        public DateTime? em_signaturedate { get; set; }
        
        public DateTime? em_incidentdate { get; set; }
        
        public string em_signatoryname { get; set; }


    }


    [DataContract(Name = "cp_pocketnotebook")]
    public class DVPocketNotebookImages : DVBase
    {

        public DVPocketNotebookImages()
        {
        }

        [DataMember]
        public string em_sketch { get; set; }

        [DataMember]
        public string em_signature { get; set; }


    }
}
