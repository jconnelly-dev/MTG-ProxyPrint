using System.Collections.Generic;


namespace ProxyPrint.DTO
{
    public class ProxyDeck
    {
        #region Members.
        public string Name { get; set; }
        public List<ProxyCard> Cards { get; set; }
        #endregion

        #region Constructors.
        public ProxyDeck(string name)
        {
            Name = name;
            Cards = new List<ProxyCard>();
        }

        public ProxyDeck(string name, List<ProxyCard> cards)
        {
            Name = name;
            Cards = cards;
        }
        #endregion
    }
}
