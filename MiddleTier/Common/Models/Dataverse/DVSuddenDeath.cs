using API.Models.Base;
using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    [DataContract(Name = "cp_suddendeath")]
    public class DVSuddenDeath : DVBase
    {
        public Guid? cp_suddendeathid { get { return base.Id; } set { base.Id = value.Value; } }

        public string cp_arealastseenalive { get; set; }

        [RelatedEntityName("cp_incident")]
        public EntityRef cp_incident { get; set; }



    }
}
