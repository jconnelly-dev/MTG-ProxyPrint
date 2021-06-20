using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MagicConsumer.WizardsAPI.DTO;

namespace MagicConsumer.WizardsAPI
{
    public class WizardsConsumer : IMagicConsumer
    {
        #region Members.
        private readonly string _downloadPath;
        private readonly string _apiCompositeUrl;
        private readonly int _apiRequestTimeout;
        #endregion

        #region Constructors.
        public WizardsConsumer(string apiDomain, string apiVersion, string apiResource, int apiRequestTimeout, string downloadPath)
        {
            _downloadPath = downloadPath;
            _apiRequestTimeout = apiRequestTimeout;
            _apiCompositeUrl = $"{apiDomain}/{apiVersion}/{apiResource}";

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

            string cardPath = DownloadCardImage(card, _downloadPath);
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

            string jsonResponse = null;

            try
            {
                // DEBUG: https://api.magicthegathering.io/v1/cards/386615

                string url = $"{_apiCompositeUrl}/{multiverseId}";
                string contentType = "application/json";
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, _apiRequestTimeout);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                    using (var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result) // forcing sync
                    {
                        response.EnsureSuccessStatusCode();
                        jsonResponse = response.Content.ReadAsStringAsync().Result; // forcing sync

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to Find Card Name. {ex.Message}");
            }

            try
            {
                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    MagicCardResponse content = JsonConvert.DeserializeObject<MagicCardResponse>(jsonResponse);
                    if (content != null && content.card != null)
                    {
                        card = content.card;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal Deserialization Error: {ex.Message}");
            }

            return card;
        }

        public List<MagicCardDTO> GetCards(string rawCardName)
        {
            List<MagicCardDTO> card = null;

            string jsonResponse = null;

            try
            {
                string cardName = rawCardName.Trim();

                // DEBUG: https://api.magicthegathering.io/v1/cards?name="Archangel%20Avacyn"

                string url = $"{_apiCompositeUrl}?name={cardName}";
                string contentType = "application/json";
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, _apiRequestTimeout);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                    using (var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result) // forcing sync
                    {
                        response.EnsureSuccessStatusCode();
                        jsonResponse = response.Content.ReadAsStringAsync().Result; // forcing sync
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to Find Card Name. {ex.Message}");
            }

            try
            {
                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    MagicCardsResponse content = JsonConvert.DeserializeObject<MagicCardsResponse>(jsonResponse);
                    if (content != null && content.cards != null && content.cards.Length > 0)
                    {
                        // all versions of the card associated w/specified name.
                        card = content.cards.ToList<MagicCardDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal Deserialization Error: {ex.Message}");
            }

            return card;
        }

        public string DownloadCardImage(MagicCardDTO card, string downloadPath)
        {
            if (card == null || string.IsNullOrEmpty(card.name) || string.IsNullOrEmpty(card.imageUrl) || string.IsNullOrEmpty(downloadPath))
            {
                return null;
            }

            string cardPath = $"{downloadPath}/{card.multiverseid}.png";
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
