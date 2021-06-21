using API.Models.Base;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace API.Models.PNB
{
    [DataContract(Name = "cp_photo")]
    public class DVPhoto : DVBase
    {

        public DVPhoto()
        {
        }

        [DataMember]
        public string cp_phototitle { get; set; }

        [JsonPropertyName("cp_PocketNotebook@odata.bind")]
        [DataMember]
        public string cp_pocketnotebook { get; set; }

        [JsonPropertyName("ownerid@odata.bind")]
        [DataMember]
        public string ownerid { get; set; }


    }


    [DataContract(Name = "cp_photo")]
    public class DVPhotoImages : DVBase
    {

        public DVPhotoImages()
        {
        }

        [DataMember]
        public string cp_image { get; set; }

    }
}
