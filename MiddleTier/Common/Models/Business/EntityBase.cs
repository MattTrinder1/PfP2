using System;

namespace Common.Models.Business
{
    public class EntityBase
    {
        private Guid? id = null;

        public EntityBase() 
        { 
        }

        public virtual Guid? Id
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

        public virtual string Name { get; set; }
    }

    public class IncidentRelatedEntityBase : EntityBase
    {
        public IncidentRelatedEntityBase()
        {
        }

        public string IncidentNumber { get; set; }

        public DateTime? IncidentDate { get; set; }
    }
}