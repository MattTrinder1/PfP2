using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class SuddenDeath : EntityBase
    {
        //text fields

        public string AreaLastSeenAlive { get; set; }
        public string NextOfKinInformedMethod { get; set; }
        public string DeathDiagnosedBy { get; set; }
        public string IdentificationLocation { get; set; }
        public string UndertakerArrangingFuneral { get; set; }
        public string FamilyLiaisonOfficer { get; set; }
        public string LastSeenAliveBy { get; set; }
        public string NextOfKinActionToInform { get; set; }
        public string CertifiedBy { get; set; }
        public string PlaceOfDeath { get; set; }
        public string BodyRemovedTo { get; set; }
        public string BodyFoundBy { get; set; }
        public string BodyLabel { get; set; }
    }
}
