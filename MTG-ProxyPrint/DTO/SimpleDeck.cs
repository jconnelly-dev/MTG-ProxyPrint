using System.Collections.Generic;


namespace ProxyPrint.DTO
{
    public class SimpleDeck
    {
        #region Members.
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
