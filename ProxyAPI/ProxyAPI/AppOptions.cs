using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI
{
    public class AppOptions
    {
        public const string Development = "DEBUG";
        public const string Release = "PROD";

        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string DownloadPath { get; set; }
        public long MaxUploadFileByteSize { get; set; }

        public void Validate()
        {
            ValidateSetting("InputPath", InputPath, () =>
                {
                    return !Directory.Exists(InputPath);
                }
            );

            ValidateSetting("OutputPath", OutputPath, () =>
                {
                    return !Directory.Exists(OutputPath);
                }
            );

            ValidateSetting("DownloadPath", DownloadPath, () =>
                {
                    return !Directory.Exists(DownloadPath);
                }
            );

            ValidateSetting("MaxFileByteSize", MaxUploadFileByteSize, () =>
                {
                    return (MaxUploadFileByteSize <= 0);
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

        private static void ValidateSetting(string configKey, long configValue, Func<bool> invalidCondition) // Overload.
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
