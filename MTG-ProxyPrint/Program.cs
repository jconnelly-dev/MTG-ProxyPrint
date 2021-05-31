using System;
using System.Collections.Generic;


namespace MTG_ProxyPrint
{
    public class Program 
    {
        private const string APP_NAME = "MTG_ProxyPrint"; 

        public static void Main(string[] args)
        {
            Console.WriteLine($"--- START - {APP_NAME} ---");

            try
            {
                // Validate connection w/card datastore.
                MTGContext context = new MTGContext();

                // Collect deck lists based on the type of request.
                ProxyRequest request = new ProxyTextFile();
                Dictionary<string, List<string>> deskLists = request.CollectDeckLists();

                // Collect card images from datastore.
                Dictionary<string, List<object>> cardImages = context.CollectCardImages(deskLists);

                // Create proxies.
                ProxyBuilder builder = new PDFProxyBuilder();
                builder.Build(cardImages);

            }
            catch (Exception ex)
            {

            }

            Console.WriteLine($"--- END - {APP_NAME} ---");
        }
    }
}