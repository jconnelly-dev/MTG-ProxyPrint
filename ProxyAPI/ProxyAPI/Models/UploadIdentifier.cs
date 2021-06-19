using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Models
{
    /// <summary>
    /// A unique identifier associated w/a decklist that has been uploaded.
    /// </summary>
    public class UploadIdentifier
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        /// <example>"7643c6b9-8d7f-40ac-a327-81e166e12a9a"</example>
        public string Identifier { get; set; }

        public UploadIdentifier(string id)
        {
            Identifier = id;
        }
    }
}
