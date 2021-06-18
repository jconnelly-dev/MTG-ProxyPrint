using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Models
{
    /// <summary>
    /// Magic the Gathering card identifier.
    /// </summary>
    public class CardIdentifier
    {
        /// <summary>
        /// Magic the Gathering card name.
        /// </summary>
        /// <example>"Ad Nauseam"</example>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(25, MinimumLength = 1)]
        public string Name { get; set; }
    }
}
