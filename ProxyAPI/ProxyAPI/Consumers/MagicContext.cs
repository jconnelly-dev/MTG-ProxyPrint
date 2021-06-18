using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.DTO;

namespace ProxyAPI.Consumers
{
    public class MagicContext
    {
        #region Members.
        private readonly string _downloadPath;
        //private readonly IMagicAPI _api;
        #endregion

        #region Constructors.
        public MagicContext(string basePath)
        {
            if (string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Error: MTGContext() Invalid Base Path.");
            }

            DirectoryInfo directory = new DirectoryInfo(basePath);
            if (directory == null || !directory.Exists)
            {
                throw new FileNotFoundException("Error: MTGContext() Download Image Directory Not Found.");
            }

            _downloadPath = basePath;

            //_api = new WizardsAPI(downloadPath);
        }
        #endregion

        public List<ProxyDeck> CollectCardImages(List<SimpleDeck> decks)
        {
            if (decks == null || decks.Count <= 0)// || _api == null)
            {
                return null;
            }

            List<ProxyDeck> result = new List<ProxyDeck>();

            foreach (SimpleDeck deck in decks)
            {
                if (deck != null && deck.Cards != null)
                {
                    ProxyDeck proxyDeck = new ProxyDeck(deck.Name);

                    // Delete directory and all contents if exists.
                    // _downloadPath...

                    // Create directory.
                    string deckPath = "todo...";

                    foreach (SimpleCard card in deck.Cards)
                    {
                        if (card != null && !string.IsNullOrEmpty(card.Name))
                        {
                            //string cardImagePath = _api.DownloadCardImage(deckPath, card.Name);
                            string cardImagePath = string.Empty;
                            if (!string.IsNullOrEmpty(cardImagePath))
                            {
                                proxyDeck.Cards.Add(new ProxyCard(cardImagePath, card.Name, card.Quantity));
                            }
                        }
                    }

                    if (proxyDeck != null && proxyDeck.Cards != null && proxyDeck.Cards.Count == deck.Cards.Count)
                    {
                        result.Add(proxyDeck);
                    }
                }
            }

            return result;
        }

        private void SendHttpApiRequest()
        {

        }
    }
}
