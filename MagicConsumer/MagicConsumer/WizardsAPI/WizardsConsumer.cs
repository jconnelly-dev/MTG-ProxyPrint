using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using MagicConsumer.WizardsAPI.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MagicConsumer.WizardsAPI
{
    public class WizardsConsumer : IMagicConsumer
    {
        #region Members.
        public IConfiguration Configuration { get; private set; }
        private readonly string _downloadPath;

        private string _apiDomainSingleton;
        public string ApiDomain
        {
            get
            {
                if (_apiDomainSingleton == null)
                {
                    const string configKey = "API-Domain";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = @"https://api.magicthegathering.io";
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    _apiDomainSingleton = configValue;
                }

                return _apiDomainSingleton;
            }
        }

        private string _apiResourceSingleton;
        public string ApiResource
        {
            get
            {
                if (_apiResourceSingleton == null)
                {
                    const string configKey = "API-Resource";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = @"cards";
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    _apiResourceSingleton = configValue;
                }

                return _apiResourceSingleton;
            }
        }

        private string _apiVersionSingleton;
        public string ApiVersion
        {
            get
            {
                if (_apiVersionSingleton == null)
                {
                    const string configKey = "API-Version";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = "v1";
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    _apiVersionSingleton = configValue;
                }

                return _apiVersionSingleton;
            }
        }

        private int? _timeoutSecondsSingleton;
        public int TimeoutSeconds
        {
            get
            {
                if (_timeoutSecondsSingleton == null)
                {
                    const string configKey = "RequestTimeoutSeconds";

                    // TODO... nuget install System.Configuration... for now just define this w/static string like below...
                    //string configValue = ConfigurationManager.AppSetting.Get(configKey);
                    string configValue = "3600";
                    if (string.IsNullOrEmpty(configValue) || string.IsNullOrWhiteSpace(configValue))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Config.");
                    }

                    int value;
                    if (!int.TryParse(configValue, out value) || value <= 0)
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Number of Seconds.");
                    }

                    _timeoutSecondsSingleton = value;
                }

                return (int)_timeoutSecondsSingleton;
            }
        }

        private string _apiBaseUrlSingleton;
        public string ApiBaseUrl
        {
            get
            {
                if (_apiBaseUrlSingleton == null)
                {
                    const string configKey = "API-BaseUrl";

                    _apiBaseUrlSingleton = $"{ApiDomain}/{ApiVersion}/{ApiResource}/";

                    if (string.IsNullOrEmpty(_apiBaseUrlSingleton) || string.IsNullOrWhiteSpace(_apiBaseUrlSingleton))
                    {
                        throw new ArgumentException($"Error: Invalid {configKey} Composite Config.");
                    }
                }

                return _apiBaseUrlSingleton;
            }
        }
        #endregion

        #region Constructors.
        public WizardsConsumer(string basePath)
        {
            //IConfigurationBuilder builder = new ConfigurationBuilder();
            //builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            //var root = bulider.Build();
            //https://stackoverflow.com/questions/46940710/getting-value-from-appsettings-json-in-net-core
            //https://stackoverflow.com/questions/38398022/access-from-class-library-to-appsetting-json-in-asp-net-core
            //var sampleConnectionString = root.GetConnectionString("your-connection-string");
            if (string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Error: WizardsAPI() Invalid Base Path.");
            }

            DirectoryInfo directory = new DirectoryInfo(basePath);
            if (directory == null || !directory.Exists)
            {
                throw new FileNotFoundException("Error: WizardsAPI() Proxy Directory Not Found.");
            }

            _downloadPath = basePath;

            // Pull and log config values.
            Console.WriteLine($"ApiDomain={ApiDomain}");
            Console.WriteLine($"ApiVersion={ApiVersion}");
            Console.WriteLine($"TimeoutSeconds={TimeoutSeconds}");
            Console.WriteLine($"ApiBaseUrl={ApiBaseUrl}");

            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.DefaultConnectionLimit = 9999;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            ValidateSimpleRequest();
        }

        private void ValidateSimpleRequest()
        {
            const int narsetEnlightenedMasterId = 386616;

            MagicCardDTO card = GetCard(narsetEnlightenedMasterId.ToString());
            if (card == null)
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Card Request Failed.");
            }

            string cardPath = DownloadImage(card);
            if (!File.Exists(cardPath))
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Image Request Failed.");
            }

            File.Delete(cardPath);
        }
        #endregion

        private MagicCardDTO GetCard(string cardName)
        {
            MagicCardDTO card = null;

            // TODO this needs to change from an int, to a card name string...
            string url = ApiBaseUrl + cardName;
            string contentType = "application/json";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, TimeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                using (var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result) // forcing sync
                {
                    response.EnsureSuccessStatusCode();

                    string jsonString = response.Content.ReadAsStringAsync().Result; // forcing sync
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        MagicCardResponse content = JsonConvert.DeserializeObject<MagicCardResponse>(jsonString);
                        if (content != null && content.card != null)
                        {
                            card = content.card;
                        }
                    }
                }
            }

            return card;
        }

        private string DownloadImage(MagicCardDTO card)
        {
            if (card == null || string.IsNullOrEmpty(card.name) || string.IsNullOrEmpty(card.imageUrl) || string.IsNullOrEmpty(_downloadPath))
            {
                return null;
            }

            string cardPath = $"{_downloadPath}/{card.name}.png";
            if (File.Exists(cardPath))
            {
                File.Delete(cardPath);
            }

            string contentType = "application/json";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, TimeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                using (Stream output = File.OpenWrite(cardPath))
                using (Stream input = client.GetStreamAsync(card.imageUrl).Result) // forcing sync
                {
                    input.CopyTo(output);
                }
            }

            return cardPath;
        }

        public string DownloadCardImage(string deckPath, string cardName)
        {
            if (string.IsNullOrEmpty(deckPath) || string.IsNullOrEmpty(cardName))
            {
                return null;
            }

            DirectoryInfo directory = new DirectoryInfo(deckPath);
            if (directory == null || !directory.Exists)
            {
                return null;
            }

            string cardImagePath = string.Empty;

            // TODO... same thing as above... Get card based on cardName.

            // Then delete file if it already exists.

            // Get card image using imageUrl. And download image.

            return cardImagePath;
        }
    }
}
