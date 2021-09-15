using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Serialization;

namespace API.Models.Dataverse
{
    [DataContract(Name = "cp_lookupfield")]
    public class DVLookupField : DVEntityBase
    {
        public DVLookupField() { }

        public Guid? cp_lookupfieldid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_displayname { get; set; }

        public string cp_id { get; set; }

        public int? cp_totalvalues { get; set; }

        public int? cp_usedin { get; set; }
    }
}
