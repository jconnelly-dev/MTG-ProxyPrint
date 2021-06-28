using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Models
{
    /// <summary>
    /// The image of a Magic the Gathering card.
    /// </summary>
    public class CardImage
    {
        /// <summary>
        /// Type of image.
        /// </summary>
        /// <example>"image/png"</example>
        public string ContentType { get; set; }

        /// <summary>
        /// The image binary data represented as a byte array.
        /// </summary>
        public byte[] FileContents { get; set; }
    }
}
