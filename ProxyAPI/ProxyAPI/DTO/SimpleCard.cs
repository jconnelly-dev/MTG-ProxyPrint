using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.DTO
{
    public class SimpleCard
    {
        public string Name { get; set; }
        public int Quantity { get; set; }

        public SimpleCard(string name)
        {
            Name = name;
            Quantity = 1;
        }

        public SimpleCard(string name, int quantity) // Overload
        {
            Name = name;
            Quantity = quantity;
        }
    }
}
