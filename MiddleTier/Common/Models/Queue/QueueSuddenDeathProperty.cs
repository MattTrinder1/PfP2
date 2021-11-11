using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Queue
{
    public class QueueSuddenDeathProperty
    {
        public string PropertyId { get; set; }
        public string IsDisposedOrRetained { get; set; }
        public string PersonAuthorisingProperty { get; set; }
        public string PhotoPropertyBlobName { get; set; }
        public string PropertyDate { get; set; }
        public string PropertyDescription { get; set; }
        public int PropertyKey { get; set; }
        public string PropertyLocation { get; set; }
        public string PropertyRelationshipToDeceased { get; set; }
        public string PropertySignatureBlobName { get; set; }
        public string Propertynumber { get; set; }
        public string Subjecttoinvestigation { get; set; }
        public string SuddenDeathKey { get; set; }
        public string exhibitnumber { get; set; }
        public string photoPropertyInternalBlobId { get; set; }
        public string propertySignatureInternalBlobId { get; set; }
        public string seizedby { get; set; }
    }
}
