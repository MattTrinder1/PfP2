using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Business
{
    public class SuddenDeath : EntityBase
    {
        public string WhereLastSeenAlive { get; set; }
        public string IncidentNumber { get; set; }
        public DateTime IncidentDate { get; set; }
    }
}
