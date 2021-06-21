using Common.Models.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.IYC
{
    public class AppPocketNotebook : AppModelBase

    {
        public List<AppPhoto> Photos { get; set; }
        public string Notes { get; set; }
        public DateTime NoteDateAndTime { get; set; }
        public string SketchId { get; set; }
        public string SignatureId { get; set; }
        public string EnteredBy { get; set; }
        public string IncidentNumber { get; set; }

        public DateTime? SignatureDateandTime { get; set; }
        public string SignatoryName { get; set; }

    }
    public class AppPhoto
    {
        public string blobId { get; set; }
        public string caption { get; set; }
    }

    
}
