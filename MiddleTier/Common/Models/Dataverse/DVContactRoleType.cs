using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("cp_contactroletype")]
    public class DVContactRoleType : DVEntityBase
    {
        public DVContactRoleType()
        {
            this.LogicalName = "cp_contactroletype";
        }

        public Guid? cp_contactroletypeid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_name { get { return this.GetAttributeValue<string>(nameof(cp_name)); } set { this.Attributes[nameof(cp_name)] = value; } }

        public OptionSetValue cp_applicationused { get { return this.GetAttributeValue<OptionSetValue>(nameof(cp_applicationused)); } set { this.Attributes[nameof(cp_applicationused)] = value; } }
    }
}
