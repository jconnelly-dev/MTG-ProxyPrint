using System.Collections.Generic;

using ProxyPrint.DTO;


namespace ProxyPrint.Outputs
{
    public interface IProxyBuilder
    {
        public bool Build(List<ProxyDeck> decks);
    }
}
