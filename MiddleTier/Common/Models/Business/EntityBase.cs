using API.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class EntityBase
    {
        private Guid? id = null;

        public EntityBase(Guid? id, string name)
        {
            Id = id;
            Name = name;
        }
        public EntityBase(Guid? id)
        {
            Id = id;
        }
        public EntityBase()
        {
        }

        public Guid? Id
        {
            get
            {
                return id;
            }

            set
            {
                if (value != Guid.Empty)
                {
                    id = value;
                }
                else
                {
                    value = null;
                }
            }
        }
        public User OwnerId { get; set; }
        public string Name { get; set; }

        //public string EnteredBy { get; set; }
    }
}