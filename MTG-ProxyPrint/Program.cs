using System;
using System.Collections.Generic;

namespace MTG_ProxyPrint
{
    public class Program 
    {
        #region Constants.
        private const string APP_NAME = "MTG_ProxyPrint";
        private const string PROXY_PATH = @"C:\TomatoSoup\jconnelly-dev\Hello\example-decks";
        #endregion

        public static void Main(string[] args)
        {
            Console.WriteLine($"--- START - {APP_NAME} ---");

            try
            {
                // Validate connection w/card datastore.
                MTGContext context = new MTGContext();

                // Collect deck lists based on the type of request.
                IProxyRequest request = new ProxyTextFile(PROXY_PATH);
                List<SimpleDeck> deskLists = request.CollectDeckLists();

                // Collect card images from datastore.
                Dictionary<string, List<object>> cardImages = context.CollectCardImages(deskLists);

                // Create proxies.
                IProxyBuilder builder = new PDFProxyBuilder();
                builder.Build(cardImages);

            }
            catch (Exception ex)
            {
                string errMsg = string.Format("Uncaught Exception: {0}", ex.Message);
                Console.WriteLine(errMsg);
            }

            Console.WriteLine($"--- END - {APP_NAME} ---");
        }
    }
}
