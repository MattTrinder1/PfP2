using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.IYC
{
    


    public class PocketNotebook : EntityBase

    {

        public PocketNotebook()
        {
            Photos = new List<Photo>();
        }

        public List<Photo> Photos { get; set; }
        public string Notes { get; set; }
        public DateTime NoteDateAndTime { get; set; }
        public string Sketch { get; set; }
        public string Signature { get; set; }
        public string IncidentNumber { get; set; }
        public DateTime? IncidentDate { get; set; }
        public DateTime? SignatureDateandTime { get; set; }
        public string SignatoryName { get; set; }
        

    }
    

}
