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

        private static string _inputPathSingleton;
        private string InputPath
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
        private string DownloadPath
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
        private string BasePath
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
            _logger.LogInformation(eventId, $"InputPath={InputPath}");
            _logger.LogInformation(eventId, $"OutputPath={OutputPath}");
            _logger.LogInformation(eventId, $"DownloadPath={DownloadPath}");
            _logger.LogInformation(eventId, $"BasePath={BasePath}");
            _logger.LogInformation("--------------------------------------------------");
        }
    }
}
