using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class Note : EntityBase
    {
        public string Attachment { get; set; }
        public string Subject { get; set; }
        public EntityReference ParentRecord { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
