using System;
using System.Collections.Generic;
using System.IO;

using ProxyPrint.DTO;


namespace ProxyPrint
{
    public class MTGContext
    {
        #region Members.
        private readonly string _downloadPath;
        
        // TODO... need to install Nuget so that this other project compiles....
        //private readonly IMagicAPI _api;
        #endregion

        #region Constructors.
        public MTGContext(string basePath)
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
