using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class ApiConfiguration
    {
        public ApiConfiguration(
            IConfiguration configuration,
            API.Mappers.MapperConfiguration mapperConfiguration)
        {
            SuppressIntegrationKeyVerification = configuration.GetValue("API:SuppressIntegrationKeyVerification", false);
            MapperConfiguration = mapperConfiguration;
        }

        public bool SuppressIntegrationKeyVerification { get; set; }

        public API.Mappers.MapperConfiguration MapperConfiguration { get; set; }
    }
}
