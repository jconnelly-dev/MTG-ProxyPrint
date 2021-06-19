using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using MagicConsumer.WizardsAPI.DTO;
using Newtonsoft.Json;

namespace MagicConsumer.WizardsAPI
{
    public class WizardsConsumer : IMagicConsumer
    {
        #region Members.
        private readonly string _apiBaseUrl;
        private readonly string _apiDomain;
        private readonly string _apiVersion;
        private readonly string _apiResource;
        private readonly int _apiRequestTimeout;
        #endregion

        #region Constructors.
        public WizardsConsumer(string apiDomain, string apiVersion, string apiResource, int apiRequestTimeout)
        {
            _apiDomain = apiDomain;
            _apiResource = apiResource;
            _apiVersion = apiVersion;
            _apiRequestTimeout = apiRequestTimeout;

            _apiBaseUrl = $"{_apiDomain}/{apiVersion}/{apiResource}";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            ValidateSimpleRequest();
        }

        private void ValidateSimpleRequest()
        {
            const int narsetEnlightenedMasterId = 386616;

            MagicCardDTO card = GetCard(narsetEnlightenedMasterId);
            if (card == null)
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Card Request Failed.");
            }

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string cardPath = DownloadImage(card, projectDirectory);
            if (!File.Exists(cardPath))
            {
                throw new HttpRequestException("Error: ValidateSimpleRequest() Simple Image Request Failed.");
            }

            File.Delete(cardPath);
        }
        #endregion

        public MagicCardDTO GetCard(int multiverseId)
        {
            MagicCardDTO card = null;

            // DEBUG: https://api.magicthegathering.io/v1/cards/386615

            string url = $"{_apiBaseUrl}/{multiverseId}";
            string contentType = "application/json";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, _apiRequestTimeout);
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

        public List<MagicCardDTO> GetCards(string cardName)
        {
            List<MagicCardDTO> card = null; // all versions of the card associated w/specified name.

            // DEBUG: https://api.magicthegathering.io/v1/cards?name="Archangel%20Avacyn"

            string htmlCardName = cardName.Trim().Replace(" ", "%20");

            string url = $"{_apiBaseUrl}?name=\"{cardName}\"";
            string contentType = "application/json";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, _apiRequestTimeout);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                using (var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result) // forcing sync
                {
                    response.EnsureSuccessStatusCode();

                    string jsonString = response.Content.ReadAsStringAsync().Result; // forcing sync
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        MagicCardsResponse content = JsonConvert.DeserializeObject<MagicCardsResponse>(jsonString);
                        if (content != null && content.cards != null && content.cards.Length > 0)
                        {
                            card = content.cards.ToList<MagicCardDTO>();
                        }
                    }
                }
            }

            return card;
        }

        private string DownloadImage(MagicCardDTO card, string downloadPath)
        {
            if (card == null || string.IsNullOrEmpty(card.name) || string.IsNullOrEmpty(card.imageUrl) || string.IsNullOrEmpty(downloadPath))
            {
                return null;
            }

            string cardPath = $"{downloadPath}/{card.name}.png";
            if (File.Exists(cardPath))
            {
                File.Delete(cardPath);
            }

            string contentType = "application/json";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, _apiRequestTimeout);
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
