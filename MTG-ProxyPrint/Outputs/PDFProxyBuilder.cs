using System;
using System.Collections.Generic;
using System.IO;

using ProxyPrint.DTO;


namespace ProxyPrint.Outputs
{
    public class PDFProxyBuilder : IProxyBuilder
    {
        #region Members.
        private string _outputPath;
        #endregion

        #region Constructors.
        public PDFProxyBuilder(string basePath)
        {
            if (string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Error: PDFProxyBuilder() Invalid Base Path.");
            }

            DirectoryInfo directory = new DirectoryInfo(basePath);
            if (directory == null || !directory.Exists)
            {
                throw new FileNotFoundException("Error: PDFProxyBuilder() Output PDF Directory Not Found.");
            }

            _outputPath = basePath;
        }
        #endregion

        public bool Build(List<ProxyDeck> decks)
        {
            bool success = true;
            if (decks == null || decks.Count <= 0 || string.IsNullOrEmpty(_outputPath))
            {
                return !success;
            }

            return success;
        }
    }
}
