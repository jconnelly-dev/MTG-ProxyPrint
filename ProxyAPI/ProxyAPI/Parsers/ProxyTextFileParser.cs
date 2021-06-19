using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.DTO;

namespace ProxyAPI.Parsers
{
    public class ProxyTextFileParser : IProxyParser
    {
        #region Members.
        private const int TEXT_MIN_FIELDS = 2;
        private const int MIN_SINGLE_CARD = 1;
        private const int MAX_SINGLE_CARD = 75;
        private const string EXPECTED_FILE_FORMAT = "*.txt";

        private readonly List<FileInfo> _textFiles;
        #endregion

        #region Contructors.
        public ProxyTextFileParser(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Error: ProxyTextFileParser() Invalid Base Path.");
            }

            DirectoryInfo directory = new DirectoryInfo(path);
            if (directory == null || !directory.Exists)
            {
                throw new FileNotFoundException("Error: ProxyTextFileParser() Proxy Directory Not Found.");
            }

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
                throw new InvalidDataException("Error: ProxyTextFileParser() No Text Files Found.");
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
                            SimpleCard card = ParseTextFileLine(line);
                            if (card != null)
                            {
                                deck.Cards.Add(card);
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

        private SimpleCard ParseTextFileLine(string line)
        {
            if (line == null)
            {
                return null;
            }

            SimpleCard parsedCard = null;

            int cardQnty = 0;
            string trimLine = line.Trim();
            string[] lineFields = (trimLine == null) ? null : trimLine.Split();
            if (lineFields != null && lineFields.Length >= TEXT_MIN_FIELDS)
            {
                if (int.TryParse(lineFields[0], out cardQnty) && cardQnty >= MIN_SINGLE_CARD && cardQnty <= MAX_SINGLE_CARD)
                {
                    // Remove card quantity field from text line so that the only remaining text is the card name.
                    string cardName = trimLine.TrimStart(lineFields[0].ToCharArray()).Trim();

                    if (!string.IsNullOrEmpty(cardName) && !string.IsNullOrWhiteSpace(cardName))
                    {
                        parsedCard = new SimpleCard(cardName, cardQnty);
                    }
                }
                //else if (lineFields[0].Trim("x"))
                //{

                //}
            }

            return parsedCard;
        }
    }
}
