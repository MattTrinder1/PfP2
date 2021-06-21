using API.Models.Base;
using System.Runtime.Serialization;

namespace API.Models.IYC
{
    [DataContract]
    public class DVItsYourChoice : DVBase
    {

        public DVItsYourChoice()
        {
        }
        [DataMember]
        public string cp_description { get; set; }

        [DataMember]
        public string cp_fulldetails { get; set; }

    }
}
