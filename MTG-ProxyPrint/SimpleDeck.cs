using System;
using System.Collections.Generic;
using System.Text;

namespace MTG_ProxyPrint
{
    public class SimpleDeck
    {
        #region Properties.
        public string Name { get; set; }
        public List<SimpleCard> Cards { get; set; }
        #endregion

        public SimpleDeck(string name)
        {
            Name = name;
            Cards = new List<SimpleCard>();
        }

        public SimpleDeck(string name, List<SimpleCard> cards)
        {
            Name = name;
            Cards = cards;
        }
    }
}
