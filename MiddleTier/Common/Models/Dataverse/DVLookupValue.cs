using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Dataverse
{
    public class DVLookupValue
    {
        public Guid cp_lookupvalueid { get; set; }
        public string cp_name { get; set; }
        public int? cp_displaysequenceno { get; set; }
    }
}
