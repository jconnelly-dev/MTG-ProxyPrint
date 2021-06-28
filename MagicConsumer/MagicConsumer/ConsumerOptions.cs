using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicConsumer
{
    public class ConsumerOptions
    {
        public const string SectionName = "WizardsOfTheCoast_API_Settings";

        public string Domain { get; set; }
        public string Version { get; set; }
        public string Resource { get; set; }
        public int RequestTimeoutSeconds { get; set; }

        public void Validate()
        {
            ValidateSetting("Domain", Domain, () => 
                {
                    return !Domain.Contains("http") && !Domain.Contains("https");
                }
            );

            ValidateSetting("Version", Version);

            ValidateSetting("Resource", Resource);

            ValidateSetting("RequestTimeoutSeconds", RequestTimeoutSeconds, () =>
                {
                    return (RequestTimeoutSeconds <= 0);
                }
            );
        }

        private static void ValidateSetting(string configKey, string configValue, Func<bool> invalidCondition)
        {
            if (string.IsNullOrEmpty(configKey) || string.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentException($"Error: Invalid {configKey} Config Key.");
            }
            if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue) || invalidCondition())
            {
                throw new ArgumentException($"Error: Invalid {configValue} Config Value.");
            }
        }

        private static void ValidateSetting(string configKey, string configValue) // Overload.
        {
            if (string.IsNullOrEmpty(configKey) || string.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentException($"Error: Invalid {configKey} Config Key.");
            }
            if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
            {
                throw new ArgumentException($"Error: Invalid {configValue} Config Value.");
            }
        }

        private static void ValidateSetting(string configKey, int configValue, Func<bool> invalidCondition) // Overload.
        {
            if (string.IsNullOrEmpty(configKey) || string.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentException($"Error: Invalid {configKey} Config Key.");
            }
            if (invalidCondition())
            {
                throw new ArgumentException($"Error: Invalid {configValue} Config Value.");
            }
        }
    }
}
