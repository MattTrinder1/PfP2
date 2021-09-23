using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_suddendeathproperty")]
    public class DVSuddenDeathProperty : DVEntityBase
    {

        public DVSuddenDeathProperty()
        {
            this.LogicalName = "cp_suddendeathproperty";
        }

        public string cp_propertyname { get { return this.GetAttributeValue<string>(nameof(cp_propertyname)); } set { this.Attributes[nameof(cp_propertyname)] = value; } }
        public string cp_actionrelatedwith { get { return this.GetAttributeValue<string>(nameof(cp_actionrelatedwith)); } set { this.Attributes[nameof(cp_actionrelatedwith)] = value; } }
        public string cp_actiontaken { get { return this.GetAttributeValue<string>(nameof(cp_actiontaken)); } set { this.Attributes[nameof(cp_actiontaken)] = value; } }
        public string cp_location { get { return this.GetAttributeValue<string>(nameof(cp_location)); } set { this.Attributes[nameof(cp_location)] = value; } }
        public string cp_relationshiptodeceased { get { return this.GetAttributeValue<string>(nameof(cp_relationshiptodeceased)); } set { this.Attributes[nameof(cp_relationshiptodeceased)] = value; } }
        public DateTime? cp_signedon { get { return this.GetAttributeValue<DateTime?>(nameof(cp_signedon)); } set { this.Attributes[nameof(cp_signedon)] = value; } }

        public EntityReference cp_suddendeath { get { return this.GetAttributeValue<EntityReference>(nameof(cp_suddendeath)); } set { this.Attributes[nameof(cp_suddendeath)] = value; } }

    }
}
