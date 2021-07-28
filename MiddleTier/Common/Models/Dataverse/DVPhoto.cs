using API.Models.Base;
using Common.Models.Business;
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

        public Guid? cp_photoid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_phototitle { get; set; }


        [RelatedEntityName("cp_pocketnotebook")]
        [JsonPropertyName("cp_PocketNotebook@odata.bind")]
        public EntityReference cp_pocketnotebook { get; set; }
        public Guid? _cp_pocketnotebook_value { get; set; }
    }

    [DataContract(Name = "cp_photo")]
    public class DVPhotoImage : DVBase
    {
        public DVPhotoImage()
        {
        }

        public Guid? cp_photoid { get { return base.Id; } set { base.Id = value.Value; } }

        [DVImage()]
        public string cp_image { get; set; }
    }
}
