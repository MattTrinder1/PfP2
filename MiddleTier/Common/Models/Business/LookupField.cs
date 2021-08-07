using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class LookupField : EntityBase
    {
        public LookupField()
        {
            Values = new List<LookupValue>();
        }

        public string FilterId { get; set; }

        public string DisplayName { get; set; }

        public int UsedIn { get; set; }

        public int TotalValues { get; set; }

        public List<LookupValue> Values { get; set; }
    }
}
