using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    public class DVEntityBase : Entity
    {
        public EntityReference ownerid { get { return this.GetAttributeValue<EntityReference>(nameof(ownerid)); } set { this.Attributes[nameof(ownerid)] = value; } }

    }
}
