using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_territorialpolicingarea")]
    public class DVTerritorialPolicingArea : DVEntityBase
    {
        public DVTerritorialPolicingArea()
        {
            this.LogicalName = "cp_territorialpolicingarea";
        }

        public Guid? cp_territorialpolicingareaid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_name { get { return this.GetAttributeValue<string>(nameof(cp_name)); } set { this.Attributes[nameof(cp_name)] = value; } }
    }
}
