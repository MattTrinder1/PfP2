using System;

namespace Common.Models.Business
{
    public class PocketNotebookPhotoDetail : EntityBase
    {
        public PocketNotebookPhotoDetail()
        {
        }

        public Guid PocketNotebookId { get; set; }
        public Guid PhotoId { get; set; }
        public string Caption { get; set; }

        
    }
}
