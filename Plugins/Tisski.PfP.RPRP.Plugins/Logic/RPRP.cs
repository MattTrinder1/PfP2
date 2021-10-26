using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tisski.PfP.RPRP.Plugins
{
    public static class RPRP
    {
        public static readonly string[] ParticipantObfuscatedAttributeNames = new string[] { "cp_additionaldetailstoreviewer", "cp_detailscircumstancesleadingtorprp", "cp_complainantsexpectations" };
        public const string ObfuscatedAttributeReplacementValue = "[You do not have permission to see this information.]";
        public const string ParticipantTeamTemplateName = "RPRP Participant";
    }
}
