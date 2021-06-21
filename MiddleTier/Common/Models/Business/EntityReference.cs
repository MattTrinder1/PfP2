using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class EntityReference
    {
        public EntityReference(string entityLogicalName, Guid? entityId, string displayValue)
        {
            EntityLogicalName = entityLogicalName;
            EntityId = entityId;
            DisplayValue = displayValue;
        }

        public EntityReference(string entityLogicalName, Guid? entityId)
        {
            EntityLogicalName = entityLogicalName;
            EntityId = entityId;
        }

        public string DisplayValue { get; set; }
        public string EntityLogicalName { get; set; }
        public Guid? EntityId { get; set; }
    }
}
