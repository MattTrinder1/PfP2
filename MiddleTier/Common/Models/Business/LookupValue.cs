using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Business
{
    public class LookupValue : EntityBase
    {
        public LookupValue(Guid id, string name) : base(id, name)
        {
        }

        public LookupValue() : base()
        {
        }


    }
}
