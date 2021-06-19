using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.DTO;

namespace ProxyAPI.Builders
{
    public interface IProxyBuilder
    {
        public bool Build(List<ProxyDeck> decks);
    }
}
