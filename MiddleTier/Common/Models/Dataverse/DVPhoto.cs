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

        public string cp_phototitle { get; set; }

        [JsonPropertyName("cp_PocketNotebook@odata.bind")]
        public string cp_pocketnotebook { get; set; }



    }


    [DataContract(Name = "cp_photo")]
    public class DVPhotoImages : DVBase
    {

        public DVPhotoImages()
        {
        }

        public string cp_image { get; set; }

    }
}
