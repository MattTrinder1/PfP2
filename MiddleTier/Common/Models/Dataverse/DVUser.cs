using Microsoft.Xrm.Sdk.Client;
using System;

namespace Common.Models.Dataverse
{
    [EntityLogicalName("systemuser")]
    public class DVUser : DVEntityBase
    {
        public DVUser()
        {
            this.LogicalName = "systemuser";
        }

        public Guid? systemuserid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_badgenumber { get { return this.GetAttributeValue<string>(nameof(cp_badgenumber)); } set { this.Attributes[nameof(cp_badgenumber)] = value; } }

        public string internalemailaddress { get { return this.GetAttributeValue<string>(nameof(internalemailaddress)); } set { this.Attributes[nameof(internalemailaddress)] = value; } }

        public string fullname { get { return this.GetAttributeValue<string>(nameof(fullname)); } set { this.Attributes[nameof(fullname)] = value; } }

        public string firstname { get { return this.GetAttributeValue<string>(nameof(firstname)); } set { this.Attributes[nameof(firstname)] = value; } }
    }
}
