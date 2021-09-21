using System;

namespace Common.Models.Business
{
    public class Photo : EntityBase
    {
        public Guid? PocketNotebookId { get; set; }
        public Guid? SuddenDeathId { get; set; }
        public string Blob { get; set; }
        public string Caption { get; set; }
    }
}
