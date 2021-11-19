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
        private string GetValue(string key, string defaultValue = null, bool throwException = false)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (value == null)
            {
                value = _configuration.GetValue<string>(key);
            }
            if (value == null)
            {
                if (throwException)
                {
                    throw new ArgumentException($"Cannot find a configuration value with key, {key}");
                }
                else
                {
                    value = defaultValue;
                }
            }
            return value;
        }

        /// <summary>
        /// Retrieves a value from process env variables, with fallback to appsettings, returning a default value if not found.
        /// </summary>
        public string GetValue(string key, string defaultValue)
        {
            return GetValue(key, defaultValue, false);
        }

        /// <summary>
        /// Retrieves a value from process env variables, with fallback to appsettings and option to throw an exception if not found.
        /// </summary>
        public string GetValue(string key, bool throwException)
        {
            return GetValue(key, null, throwException);
        }

        /// <summary>
        /// Retrieves a value from process env variables, with fallback to appsettings, returning null if not found.
        /// </summary>
        public string GetValue(string key)
        {
            return GetValue(key, null, false);
        }
    }
}
