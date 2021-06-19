using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Models
{
    /// <summary>
    /// An identifier for an uploaded decklist.
    /// </summary>
    public class UploadIdentifier
    {
        /// <summary>
        /// Unique card name.
        /// </summary>
        /// <example>"7643c6b9-8d7f-40ac-a327-81e166e12a9a"</example>
        public string Identifier;
    }
}
