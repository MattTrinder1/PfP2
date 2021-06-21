using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.IYC
{
    
    public class PocketNotebookListEntry :EntityBase

    {
        public PocketNotebookListEntry()
        {
        }

        public string Notes { get; set; }
        public DateTime NoteDateAndTime { get; set; }
    }


}
