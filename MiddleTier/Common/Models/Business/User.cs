using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Business
{
    public class User : EntityBase
    {
        public User(Guid id, string name) : base(id, name)
        {
        }

        public User(Guid id) : base(id)
        {
        }
        public User() : base()
        {
        }


    }
}
