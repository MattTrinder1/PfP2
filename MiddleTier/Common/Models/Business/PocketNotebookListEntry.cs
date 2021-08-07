using System;

namespace Common.Models.Business
{
    public class PocketNotebookListEntry : EntityBase
    {
        public PocketNotebookListEntry()
        {
        }

        public string Notes { get; set; }

        public DateTime NoteDateAndTime { get; set; }
    }
}
