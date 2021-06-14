using System.Collections.Generic;

using ProxyPrint.DTO;


namespace ProxyPrint.Inputs
{
    public interface IProxyRequest
    {
        List<SimpleDeck> CollectDeckLists();
    }
}
