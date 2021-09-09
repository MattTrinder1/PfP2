using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseRepository
{
    public class ConnectionConfiguration
    {
        public ConnectionConfiguration(string dvUrl, string clientId, string clientSecret)
        {
            DVUrl = dvUrl;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string DVUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
