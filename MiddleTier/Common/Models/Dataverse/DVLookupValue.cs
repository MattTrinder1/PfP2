using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Serialization;

namespace API.Models.Dataverse
{
    [DataContract(Name = "cp_lookupvalue")]
    public class DVLookupValue : Entity
    {
        public DVLookupValue() { }

        public Guid? cp_lookupvalueid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_name { get; set; }

        public string cp_alternativedisplayname { get; set; }

        public int? cp_displaysequenceno { get; set; }
    }
}
