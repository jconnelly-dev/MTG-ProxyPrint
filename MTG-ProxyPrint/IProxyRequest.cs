using System;
using System.Collections.Generic;
using System.IO;

namespace MTG_ProxyPrint
{
    public interface IProxyRequest
    {
        List<SimpleDeck> CollectDeckLists();
    }

    public class ProxyTextFile : IProxyRequest
    {
        #region Constants.
        private const string EXPECTED_FILE_FORMAT = "*.txt";
        private const int TEXT_MIN_FIELDS = 2;
        private const int MIN_SINGLE_CARD = 1;
        private const int MAX_SINGLE_CARD = 4;
        #endregion

        #region Members.
        private List<FileInfo> _textFiles;
        #endregion

        public ProxyTextFile(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Error: ProxyTextFile() Invalid Arguments.");
            }

            DirectoryInfo directory = new DirectoryInfo(path);
            if (directory == null || !directory.Exists)
            {
                throw new ArgumentException("Error: ProxyTextFile() Invalid Directory.");
            }

            this._textFiles = new List<FileInfo>();
            FileInfo[] files = directory.GetFiles(EXPECTED_FILE_FORMAT);
            if (files != null && files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    if (file != null && !string.IsNullOrEmpty(file.Name))
                    {
                        this._textFiles.Add(file);
                    }
                }
            }
            if (this._textFiles == null || this._textFiles.Count <= 0)
            {
                throw new ArgumentException("Error: ProxyTextFile() No Text Files Found.");
            }    
        }

        public List<SimpleDeck> CollectDeckLists()
        {
            if (this._textFiles == null || this._textFiles.Count <= 0)
            {
                return null;
            }

            List<SimpleDeck> decks = new List<SimpleDeck>();

            foreach (FileInfo file in this._textFiles)
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
