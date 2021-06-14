using System;
using System.Collections.Generic;
using System.Net.Http;


namespace MTG_ProxyPrint
{
    public class MTGContext
    {
        #region Members.
        private readonly IMagicAPI _api;
        #endregion

        #region Constructors.
        public MTGContext(string basePath)
        {
            _api = new MagicTheGatheringAPI(basePath);
        }
        #endregion

        public Dictionary<string, List<object>> CollectCardImages(List<SimpleDeck> decks)
        {
            if (decks == null || decks.Count <= 0 || _api == null)
            {
                return null;
            }

            Dictionary<string, List<object>> result = new Dictionary<string, List<object>>();

            _api.SendRequest();

            return result;
        }

        private void SendHttpApiRequest()
        {

        }
    }
}
