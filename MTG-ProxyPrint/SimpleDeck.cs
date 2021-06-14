using System.Collections.Generic;


namespace MTG_ProxyPrint
{
    public class SimpleDeck
    {
        #region Properties.
        public string Name { get; set; }
        public List<SimpleCard> Cards { get; set; }
        #endregion

        #region Constructors.
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
        #endregion
    }
}
