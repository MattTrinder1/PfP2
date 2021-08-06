using System;
using System.Collections.Generic;

namespace Common.Models.Business
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
