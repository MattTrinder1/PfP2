using API.Models.Business;
using System;

namespace Common.Models.Business
{
    public class EntityBase
    {
        private Guid? id = null;

        public EntityBase()
        {
        }

        public EntityBase(Guid? id, string name)
        {
            Id = id;
            Name = name;
        }
        public EntityBase(Guid? id)
        {
            Id = id;
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

        public string Name { get; set; }
        public string IncidentNumber { get; set; }

        public DateTime? IncidentDate { get; set; }


        //public User OwnerId { get; set; }
    }
}