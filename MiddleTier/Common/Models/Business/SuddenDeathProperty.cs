using System;

namespace Common.Models.Business
{
    public class SuddenDeathProperty : EntityBase
    {
        public string IsDisposedOrRetained { get; set; }
        public string PersonAuthorisingProperty { get; set; }
        public string PhotoProperty{ get; set; }
        public DateTime? PropertyDate { get; set; }
        public string PropertyDescription { get; set; }
        public int PropertyKey { get; set; }
        public string PropertyLocation { get; set; }
        public string PropertyRelationshipToDeceased { get; set; }
        public string PropertySignature{ get; set; }
        public string Propertynumber { get; set; }
        public string Subjecttoinvestigation { get; set; }
        public string exhibitnumber { get; set; }
        public string seizedby { get; set; }
    }
}
