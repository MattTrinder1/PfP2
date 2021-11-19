using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class ApiConfiguration
    {
        private IConfiguration _configuration = null;

        public ApiConfiguration(
            IConfiguration configuration,
            API.Mappers.MapperConfig mapperConfiguration)
        {
            _configuration = configuration;
            SuppressIntegrationKeyVerification = configuration.GetValue("API:SuppressIntegrationKeyVerification", false);
            MapperConfiguration = mapperConfiguration;
        }

        public bool SuppressIntegrationKeyVerification { get; set; }

        public API.Mappers.MapperConfig MapperConfiguration { get; set; }

        /// <summary>
        /// Retrieves a value from process env variables, with fallback to appsettings.
        /// </summary>
        public string GetValue(string key, string defaultValue = null)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (value == null)
            {
                value = _configuration.GetValue<string>(key);
            }
            if (value == null)
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
