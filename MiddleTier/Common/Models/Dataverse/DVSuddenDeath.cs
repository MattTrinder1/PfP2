using API.Models.Base;
using Common.Models.Business;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    [DataContract(Name = "cp_suddendeath")]
    public class DVSuddenDeath : Entity// DVBase
    {
        public Guid? cp_suddendeathid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_arealastseenalive { get; set; }

        [RelatedEntityName("cp_incident")]
        public EntityRef cp_incident { get; set; }
        
        public string cp_nextofkininformedmethod { get; set; }
        public string cp_deathdiagnosedby { get; set; }
        public string cp_identificationlocation { get; set; }
        public string cp_undertakerarrangingfuneral { get; set; }
        public string cp_sdfamilylaisonofficer { get; set; }
        public string cp_sdlastseenaliveby { get; set; }
        public string cp_nextofkinactiontoinform { get; set; }
        public string cp_certifiedby { get; set; }
        public string cp_placeofdeath { get; set; }
        public string cp_bodyremovedto { get; set; }
        public string cp_bodyfoundby { get; set; }
        public string cp_bodylabel { get; set; }


    }
}
