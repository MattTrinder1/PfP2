using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class LookupValue : EntityBase
    {
        public string AlternativeDisplayName { get; set; }

        public int DisplaySequenceNo { get; set; }                
    }
}
