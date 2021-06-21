using API.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    [DataContract(Name = "annotation")]

    public class DVNote : DVBase
    {
        public string subject { get; set; }
        public string documentbody { get; set; }
        public string mimetype { get; set; }
        public string filename { get; set; }
    }
}
