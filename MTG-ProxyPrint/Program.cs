using System;
using System.Collections.Generic;
// TODO... nuget install System.Configuration... 
//using System.Configuration;
//using System.Collections.Specialized;
using System.IO;
using System.Net.Http;

using ProxyPrint.DTO;
using ProxyPrint.Inputs;
using ProxyPrint.Outputs;


namespace ProxyPrint
{
    public class Program 
    {
        #region Members.
        private const string APP_NAME = "ProxyPrint";

        private static int? _trackingIdSingleton;
        private static int TrackingId
        {
            get
            {
                if (_trackingIdSingleton == null)
                {
                    Random rand = new Random();
                    _trackingIdSingleton = rand.Next(0, int.MaxValue);
                    if (_trackingIdSingleton < 0)
                    {
                        throw new ArgumentException($"Error: Unable to Create TrackingId."); 
                    }
                }

                return (int)_trackingIdSingleton;
            }
        }

        private static string _outputPathSingleton;
        private static string OutputPath
        {
            get
            {
                if (_outputPathSingleton == null)
                {
                    const string configKey = "OutputPath";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = "PDF";
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
        private static string DownloadPath
        {
            get
            {
                if (_downloadPathSingleton == null)
                {
                    const string configKey = "DownloadPath";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = @"Images";
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

        private static string _inputPathSingleton;
        private static string InputPath
        {
            get
            {
                if (_inputPathSingleton == null)
                {
                    const string configKey = "InputPath";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = @"Uploads";
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

        private static string _basePathSingleton;
        private static string BasePath
        {
            get
            {
                if (_basePathSingleton == null)
                {
                    const string configKey = "BasePath";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = @"C:\TomatoSoup\jconnelly-dev\MTG-Proxy-App\";
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

        public static void Main(string[] args)
        {
            Console.WriteLine($"-----------------------------------");
            Console.WriteLine($"Application={APP_NAME}");

            try
            {
                // Pull and log config values.
                Console.WriteLine($"TrackingId={TrackingId}");
                Console.WriteLine($"BasePath={BasePath}");                
                Console.WriteLine($"InputPath={InputPath}");
                Console.WriteLine($"OutputPath={OutputPath}");
                Console.WriteLine($"DownloadPath={DownloadPath}");
                Console.WriteLine($"-----------------------------------");

                // Collect deck lists based on the type of request input.
                IProxyRequest request = new ProxyTextFile(InputPath);
                List<SimpleDeck> deskLists = request.CollectDeckLists();

                // Validate connection w/card datastore.
                MTGContext context = new MTGContext(DownloadPath);

                // Collect card images from datastore.
                List<ProxyDeck> cardImages = context.CollectCardImages(deskLists);

                // Create proxies.
                IProxyBuilder builder = new PDFProxyBuilder(OutputPath);
                builder.Build(cardImages);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine($"ArgumentException: {ae.Message}");
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine($"FileNotFoundException: {fne.Message}");
            }
            catch (InvalidDataException ide)
            {
                Console.WriteLine($"InvalidDataException: {ide.Message}");
            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine($"HttpRequestException: {hre.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UncaughtException: {ex.Message}");
            }

            Console.WriteLine($"End -> {APP_NAME}");
            Console.WriteLine($"-----------------------------------");
        }
    }
}
