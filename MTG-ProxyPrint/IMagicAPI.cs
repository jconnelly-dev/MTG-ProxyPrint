using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace MTG_ProxyPrint
{
    public interface IMagicAPI
    {
        public void SendRequest();
    }

    #region Magic DTOs.
    public class MagicCardResponse
    {
        public MagicCardDTO card;
    }

    public class MagicCardDTO
    {
        public string name;
        public string manaCost;
        public string cmc;
        public string[] colors;
        public string[] colorIdentity;
        public string type;
        public string[] supertypes;
        public string[] types;
        public string[] subtypes;
        public string rarity;
        public string set;
        public string setName;
        public string text;
        public string artist;
        public string number;
        public string power;
        public string toughness;
        public string layout;
        public int multiverseid;
        public string imageUrl;
        public string watermark;
        public MagicRulingDTO[] rulings;
        public MagicForeignNamesDTO[] foreignNames;
        public string[] printings;
        public string originalText;
        public string originalType;
        public MagicLegalityDTO[] legalities;
        public string id;
    }

    public class MagicRulingDTO
    {
        public string date;
        public string text;
    }

    public class MagicForeignNamesDTO
    {
        public string name;
        public string text;
        public string type;
        public string flavor;
        public string imageUrl;
        public string language;
        public int multiverseid;
    }

    public class MagicLegalityDTO
    {
        public string format;
        public string legality;
    }
    #endregion

    public class MagicTheGatheringAPI : IMagicAPI
    {
        #region Members.
        private const string API_RESOURCE = "cards";
        private readonly string _apiDomain;
        private readonly string _apiBaseUrl;
        private readonly string _apiVersion;
        private readonly int _timeoutSeconds;
        #endregion

        #region Constructors.
        public MagicTheGatheringAPI(string basePath)
        {
            if (string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Error: MagicTheGatheringAPI() Invalid Base Path.");
            }

            InitFromConfigFile(out _apiDomain, out _apiVersion, out _timeoutSeconds);
            _apiBaseUrl = $"{_apiDomain}/{_apiVersion}/{API_RESOURCE}/";

            ValidateSimpleRequest(basePath);
        }

        private void InitFromConfigFile(out string apiDomain, out string apiVersion, out int timeoutSeconds)
        {
            apiDomain = $"https://api.magicthegathering.io";
            apiVersion = "v1";
            timeoutSeconds = 3600;
        }

        private void ValidateSimpleRequest(string baseFilePath)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            const int narsetEnlightenedMasterId = 386616;

            MagicCardDTO card = null;
            string contentType = "application/json";
            string url = _apiBaseUrl + narsetEnlightenedMasterId.ToString();
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, _timeoutSeconds);
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
            if (card == null)
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Card Request Failed.");
            }

            string cardPath = baseFilePath + @"\Images\" + card.multiverseid.ToString() + ".png";
            if (File.Exists(cardPath))
            {
                File.Delete(cardPath);
            }

            //string imageUrl = _protocol + @"gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseid.ToString() + "&type=card";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, _timeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                using (Stream output = File.OpenWrite(cardPath))
                using (Stream input = client.GetStreamAsync(card.imageUrl).Result) // forcing sync
                {
                    input.CopyTo(output);
                }
            }
            if (!File.Exists(cardPath))
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Image Request Failed.");
            }

            File.Delete(cardPath);
        }

        private async void ValidateSimpleRequest_POST_NO_WORK()
        {
            const int narsetEnlightenedMasterId = 386616;
            const string restType = "GET";
            const string contentType = "application/json";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            bool valid = false;
            string requestBody = string.Empty;
            string bodyContents = JsonConvert.SerializeObject(requestBody);
            string url = _apiBaseUrl + narsetEnlightenedMasterId.ToString();
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, _timeoutSeconds);
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                HttpMethod method = new HttpMethod(restType);
                HttpContent body = new StringContent(bodyContents, Encoding.UTF8, contentType);

                using (HttpResponseMessage response = client.PostAsync(url, body).Result) // forcing sync
                {
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result; // forcing sync
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            MagicCardDTO content = JsonConvert.DeserializeObject<MagicCardDTO>(jsonString);
                            if (content != null)
                            {
                                valid = true;
                            }
                        }
                    }
                }
            }

            if (!valid)
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Request Failed.");
            }
        }
        #endregion

        public void SendRequest()
        {

        }
    }
}
