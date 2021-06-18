using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.DTO
{
    public class ProxyCard : SimpleCard
    {
        public string ImagePath { get; set; }

        public ProxyCard(string imagePath, string name) : base(name)
        {
            ImagePath = imagePath;
        }

        public ProxyCard(string imagePath, string name, int quantity) : base(name, quantity) // Overload
        {
            ImagePath = imagePath;
        }
    }
}
