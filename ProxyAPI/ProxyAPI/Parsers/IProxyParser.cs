using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.DTO;

namespace ProxyAPI.Parsers
{
    public interface IProxyParser
    {
        public List<SimpleDeck> CollectDeckLists();
    }
}
