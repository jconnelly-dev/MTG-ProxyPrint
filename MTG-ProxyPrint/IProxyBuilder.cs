using System;
using System.Collections.Generic;

namespace MTG_ProxyPrint
{
    public interface IProxyBuilder
    {
        public bool Build(Dictionary<string, List<object>> decks);
    }

    public class PDFProxyBuilder : IProxyBuilder
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
