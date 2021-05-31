using System;
using System.Collections.Generic;


namespace MTG_ProxyPrint
{
    public interface ProxyBuilder
    {
        public bool Build(Dictionary<string, List<object>> decks);
    }

    public class PDFProxyBuilder : ProxyBuilder
    {
        public PDFProxyBuilder()
        {

        }

        public bool Build(Dictionary<string, List<object>> decks)
        {
            bool success = true;
            if (decks == null || decks.Count <= 0)
            {
                return !success;
            }

            return success;
        }
    }
}