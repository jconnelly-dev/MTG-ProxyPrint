using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.DTO
{
    public class ProxyDeck
    {
        public string Name { get; set; }
        public List<ProxyCard> Cards { get; set; }

        public ProxyDeck(string name)
        {
            Name = name;
            Cards = new List<ProxyCard>();
        }

        public ProxyDeck(string name, List<ProxyCard> cards) // Overload
        {
            Name = name;
            Cards = cards;
        }
    }
}
