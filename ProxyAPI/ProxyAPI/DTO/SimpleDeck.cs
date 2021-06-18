using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.DTO
{
    public class SimpleDeck
    {
        public string Name { get; set; }
        public List<SimpleCard> Cards { get; set; }

        public SimpleDeck(string name)
        {
            Name = name;
            Cards = new List<SimpleCard>();
        }

        public SimpleDeck(string name, List<SimpleCard> cards) // Overload
        {
            Name = name;
            Cards = cards;
        }
    }
}
