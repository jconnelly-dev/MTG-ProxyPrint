using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.DTO;

namespace ProxyAPI.Inputs
{
    public interface IProxyRequest
    {
        public List<SimpleDeck> CollectDeckLists();
    }
}
