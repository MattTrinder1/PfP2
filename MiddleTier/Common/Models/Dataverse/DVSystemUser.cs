using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Models.Dataverse
{
    [DataContract]
    public class DVSystemUser
    {
        [DataMember]
        public Guid SystemUserId { get; set; }
    }
}
