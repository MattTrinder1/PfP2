using System;

namespace Common.Models.Business
{
    public class PocketNotebookListEntry : IncidentRelatedEntityBase
    {
        public PocketNotebookListEntry()
        {
        }

        public string Notes { get; set; }

        public DateTime NoteDateAndTime { get; set; }

        public int PhotoCount { get; set; }
    }
}
