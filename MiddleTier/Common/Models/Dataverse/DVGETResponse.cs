using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Common.Models.Dataverse
{
    [DataContract]
    public class DVGETResponse<T>
    {
        [DataMember]
        [JsonPropertyName("@odata.context")]
        public string context { get; set; }

        public ICollection<T> value { get; set; }

    }
}
