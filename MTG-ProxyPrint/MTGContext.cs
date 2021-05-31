using System;
using System.Collections.Generic;


namespace MTG_ProxyPrint
{
    public class MTGContext
    {
        public MTGContext()
        {
            ValidationConnection();
        }

        private bool ValidationConnection()
        {
            return false;
        }

        public Dictionary<string, List<object>> CollectCardImages(Dictionary<string, List<string>> cards)
        {
            if (cards == null || cards.Count <= 0)
            {
                return null;
            }

            Dictionary<string, List<object>> result = new Dictionary<string, List<object>>();

            return result;
        }
    }
}