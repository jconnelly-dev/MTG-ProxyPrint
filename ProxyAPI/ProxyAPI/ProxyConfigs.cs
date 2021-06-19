using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI
{
    public class ProxyConfigs
    {
        private readonly IConfiguration _configs;
        private readonly ILogger<ProxyConfigs> _logger;

        #region ProxyAPI Configurations.
        private static string _inputPathSingleton;
        public string InputPath
        {
            get
            {
                if (_inputPathSingleton == null)
                {
                    const string configKey = "InputPath";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    string configPath = Path.Combine(BasePath, configValue);
                    if (!Directory.Exists(configPath))
                    {
                        throw new ArgumentException($"Error: {configKey} Directory Not Found.");
                    }

                    _inputPathSingleton = configPath;
                }

                return _inputPathSingleton;
            }
        }

        private static string _outputPathSingleton;
        public string OutputPath
        {
            get
            {
                if (_outputPathSingleton == null)
                {
                    const string configKey = "OutputPath";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    string configPath = Path.Combine(BasePath, configValue);
                    if (!Directory.Exists(configPath))
                    {
                        throw new ArgumentException($"Error: {configKey} Directory Not Found.");
                    }

                    _outputPathSingleton = configPath;
                }

                return _outputPathSingleton;
            }
        }

        private static string _downloadPathSingleton;
        public string DownloadPath
        {
            get
            {
                if (_downloadPathSingleton == null)
                {
                    const string configKey = "DownloadPath";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    string configPath = Path.Combine(BasePath, configValue);
                    if (!Directory.Exists(configPath))
                    {
                        throw new ArgumentException($"Error: {configKey} Directory Not Found.");
                    }

                    _downloadPathSingleton = configPath;
                }

                return _downloadPathSingleton;
            }
        }

        private static string _basePathSingleton;
        public string BasePath
        {
            get
            {
                if (_basePathSingleton == null)
                {
                    const string configKey = "BasePath";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    if (!Directory.Exists(configValue))
                    {
                        throw new ArgumentException($"Error: {configKey} Directory Not Found.");
                    }

                    _basePathSingleton = configValue;
                }

                return _basePathSingleton;
            }
        }
        #endregion

        #region MagicConsumer Configurations.
        private static string _apiDomainSingleton;
        public string ApiDomain
        {
            get
            {
                if (_apiDomainSingleton == null)
                {
                    const string configKey = "API-Domain";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    if (!configValue.Contains("http") && !configValue.Contains("https"))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Web Address.");
                    }

                    _apiDomainSingleton = configValue;
                }

                return _apiDomainSingleton;
            }
        }

        private static string _apiVersionSingleton;
        public string ApiVersion
        {
            get
            {
                if (_apiVersionSingleton == null)
                {
                    const string configKey = "API-Version";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    _apiVersionSingleton = configValue;
                }

                return _apiVersionSingleton;
            }
        }

        private static string _apiResourceSingleton;
        public string ApiResource
        {
            get
            {
                if (_apiResourceSingleton == null)
                {
                    const string configKey = "API-Resource";

                    string configValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    _apiResourceSingleton = configValue;
                }

                return _apiResourceSingleton;
            }
        }

        private static int? _apiRequestTimeoutSeconds;
        public int ApiRequestTimeout
        {
            get
            {
                if (_apiRequestTimeoutSeconds == null)
                {
                    const string configKey = "API-RequestTimeoutSeconds";

                    string configStrValue = _configs[configKey];
                    if (string.IsNullOrEmpty(configStrValue) || string.IsNullOrWhiteSpace(configStrValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    int configValue = 0;
                    if (!int.TryParse(configStrValue, out configValue) || configValue <= 0)
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Must be a None Zero Value.");
                    }

                    _apiRequestTimeoutSeconds = (int)configValue;
                }

                return (int)_apiRequestTimeoutSeconds;
            }
        }
        #endregion

        public ProxyConfigs(IConfiguration configs, ILogger<ProxyConfigs> logger, string projectName, string projectVersion, string machineName)
        {
            _logger = logger;
            _configs = configs;

            EventId eventId = new EventId(0, "ProxyConfigs()");

            _logger.LogInformation("--------------------------------------------------");
            _logger.LogInformation(eventId, "Initializing API...");
            _logger.LogInformation(eventId, $"ProjectName={projectName}");
            _logger.LogInformation(eventId, $"ProjectVersion={projectVersion}");
            _logger.LogInformation(eventId, $"MachineName={machineName}");
            _logger.LogInformation(eventId, "API Configurations ->");
            _logger.LogInformation(eventId, $"InputPath={InputPath}");
            _logger.LogInformation(eventId, $"OutputPath={OutputPath}");
            _logger.LogInformation(eventId, $"DownloadPath={DownloadPath}");
            _logger.LogInformation(eventId, $"BasePath={BasePath}");
            _logger.LogInformation(eventId, "Consumer Configurations ->");
            _logger.LogInformation(eventId, $"ApiDomain={ApiDomain}");
            _logger.LogInformation(eventId, $"ApiResource={ApiResource}");
            _logger.LogInformation(eventId, $"ApiVersion={ApiVersion}");
            _logger.LogInformation(eventId, $"ApiRequestTimeout={ApiRequestTimeout}");
            _logger.LogInformation("--------------------------------------------------");
        }
    }
}
