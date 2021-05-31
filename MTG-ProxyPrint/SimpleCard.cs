using System;
using System.Collections.Generic;
using System.Text;

namespace MTG_ProxyPrint
{
    public class SimpleCard
    {
        #region Properties.
        public string Name { get; set; }
        public int Quantity { get; set; }
        #endregion

        public SimpleCard(string name)
        {
            Name = name;
            Quantity = 1;
        }

        public SimpleCard(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }
}
