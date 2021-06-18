using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.DTO;

namespace ProxyAPI.Inputs
{
    public class ProxyTextFile : IProxyRequest
    {
        #region Members.
        private const string EXPECTED_FILE_FORMAT = "*.txt";
        private const int TEXT_MIN_FIELDS = 2;
        private const int MIN_SINGLE_CARD = 1;
        private const int MAX_SINGLE_CARD = 75;
        private readonly List<FileInfo> _textFiles;
        private readonly string _proxyPath;
        #endregion

        #region Contructors.
        public ProxyTextFile(string basePath)
        {
            if (string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("Error: ProxyTextFile() Invalid Base Path.");
            }

            DirectoryInfo directory = new DirectoryInfo(basePath);
            if (directory == null || !directory.Exists)
            {
                throw new FileNotFoundException("Error: ProxyTextFile() Proxy Directory Not Found.");
            }

            _proxyPath = basePath;

            _textFiles = new List<FileInfo>();
            FileInfo[] files = directory.GetFiles(EXPECTED_FILE_FORMAT);
            if (files != null && files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    if (file != null && !string.IsNullOrEmpty(file.Name))
                    {
                        _textFiles.Add(file);
                    }
                }
            }
            if (_textFiles == null || _textFiles.Count <= 0)
            {
                throw new InvalidDataException("Error: ProxyTextFile() No Text Files Found.");
            }
        }
        #endregion

        public List<SimpleDeck> CollectDeckLists()
        {
            if (_textFiles == null || _textFiles.Count <= 0)
            {
                return null;
            }

            List<SimpleDeck> decks = new List<SimpleDeck>();

            foreach (FileInfo file in _textFiles)
            {
                if (file != null && !string.IsNullOrEmpty(file.Name) && file.Exists)
                {
                    SimpleDeck deck = new SimpleDeck(file.Name);

                    using (StreamReader reader = file.OpenText())
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            int cardQnty = 0;
                            string trimLine = line.Trim();
                            string[] lineFields = (trimLine == null) ? null : trimLine.Split();
                            if (lineFields != null && lineFields.Length >= TEXT_MIN_FIELDS &&
                                int.TryParse(lineFields[0], out cardQnty) && cardQnty >= MIN_SINGLE_CARD && cardQnty <= MAX_SINGLE_CARD)
                            {
                                // Remove card quantity field from text line for the card name.
                                string cardName = trimLine.TrimStart(lineFields[0].ToCharArray()).Trim();

                                if (!string.IsNullOrEmpty(cardName) && !string.IsNullOrWhiteSpace(cardName))
                                {
                                    deck.Cards.Add(new SimpleCard(cardName, cardQnty));
                                }
                            }
                        }
                    }

                    if (deck.Cards.Count > 0)
                    {
                        decks.Add(deck);
                    }
                }
            }

            return decks;
        }
    }
}
