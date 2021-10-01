using Microsoft.Xrm.Sdk;

namespace Common.Models.Business
{
    public class Note : IncidentRelatedEntityBase
    {
        public string Attachment { get; set; }
        public string Subject { get; set; }
        public EntityReference ParentRecord { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
