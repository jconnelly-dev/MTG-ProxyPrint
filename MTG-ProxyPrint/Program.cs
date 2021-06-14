using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace MTG_ProxyPrint
{
    public class Program 
    {
        #region Constants.
        private const string APP_NAME = "MTG_ProxyPrint";
        private const string BASE_PATH = @"C:\TomatoSoup\jconnelly-dev\MTG-Proxy-App\";
        #endregion

        public static void Main(string[] args)
        {
            Console.WriteLine($"--- START - {APP_NAME} ---");

            try
            {
                // Validate connection w/card datastore.
                MTGContext context = new MTGContext(BASE_PATH);

                // Collect deck lists based on the type of request.
                IProxyRequest request = new ProxyTextFile(BASE_PATH);
                List<SimpleDeck> deskLists = request.CollectDeckLists();

                // Collect card images from datastore.
                //Dictionary<string, List<object>> cardImages = context.CollectCardImages(deskLists);

                // Create proxies.
                //IProxyBuilder builder = new PDFProxyBuilder();
                //builder.Build(cardImages);

            }
            catch (ArgumentException ae)
            {
                Console.WriteLine($"ArgumentException: {ae.Message}");
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine($"FileNotFoundException: {fne.Message}");
            }
            catch (InvalidDataException ide)
            {
                Console.WriteLine($"InvalidDataException: {ide.Message}");
            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine($"HttpRequestException: {hre.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UncaughtException: {ex.Message}");
            }

            Console.WriteLine($"--- END - {APP_NAME} ---");
        }
    }
}
