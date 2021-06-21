using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class Photo : EntityBase
    {
        public Guid? PocketNotebookId { get; set; }
        public string Blob { get; set; }
        public string Caption { get; set; }

    }
}
