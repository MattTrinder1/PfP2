using API.Models.Base;
using Common.Models.Business;
using System;
using System.Runtime.Serialization;

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
        public EntityReference cp_pocketnotebook { get; set; }

        [DVImage()]
        public string cp_image { get; set; }
    }
}
