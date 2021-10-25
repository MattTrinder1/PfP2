using System.Collections.Generic;

namespace Common.Models.Business
{
    public class LookupField : EntityBase
    {
        public LookupField()
        {
            Values = new List<LookupValue>();
        }

        public LookupField(List<LookupValue> values)
        {
            Values = new List<LookupValue>(values);
        }
        public string DisplayName { get; set; }

        public List<LookupValue> Values { get; set; }
    }
}
