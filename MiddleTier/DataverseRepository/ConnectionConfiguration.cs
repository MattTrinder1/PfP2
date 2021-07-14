using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseRepository
{
    public class ConnectionConfiguration
    {
        public ConnectionConfiguration(string dvUrl, string serviceUrl, string clientId, string clientSecret, string authority)
        {
            DVUrl = dvUrl;
            ServiceUrl = serviceUrl;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Authority = authority;
        }

        public string DVUrl { get; set; }
        public string ServiceUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
    }
}
