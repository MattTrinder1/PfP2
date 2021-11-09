using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models.Queue
{
    public class QueuePocketNotebook
    {
        public string Id { get; set; }
        public string Notes { get; set; }

        public string NoteDateAndTime { get; set; }

        [JsonPropertyName("Signature Date and Time")]
        public string SignatureDateAndTime { get; set; }

        public string IncidentDateTime { get; set; }
        public string IncidentNumber { get; set; }
        public string SignatoryName { get; set; }
        public string EnteredBy { get; set; }
        [JsonPropertyName("complete")]
        public bool Complete { get; set; }
        public int photoCount { get; set; }
        public bool hasSketch { get; set; }
        public bool hasSignature { get; set; }
    }
}
