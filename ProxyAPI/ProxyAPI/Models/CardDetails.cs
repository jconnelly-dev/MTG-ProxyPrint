using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Models
{
    /// <summary>
    /// The complete description of a Magic the Gathering card.
    /// </summary>
    public class CardDetails
    {
        /// <summary>
        /// Unique card name.
        /// </summary>
        /// <example>"Ad Nauseam"</example>
        public string Name { get; set; }

        /// <summary>
        /// Mana Cost of this card.
        /// </summary>
        /// <example>"{3}{B}{B}"</example>
        public string ManaCost { get; set; }

        /// <summary>
        /// Converted Mana Cost of this card.
        /// </summary>
        /// <example>"5.0"</example>
        public string CMC { get; set; }

        /// <summary>
        /// Colors of this card.
        /// </summary>
        /// <example>["Black"]</example>
        public List<string> Colors { get; set; }

        /// <summary>
        /// Color Identity of this card.
        /// </summary>
        /// <example>["B"]</example>
        public List<string> ColorIdentity { get; set; }

        /// <summary>
        /// Type of this card.
        /// </summary>
        /// <example>"Instant"</example>
        public string Type { get; set; }

        /// <summary>
        /// Types of this card.
        /// </summary>
        /// <example>["Instant"]</example>
        public List<string> Types { get; set; }

        /// <summary>
        /// Subtypes of this card.
        /// </summary>
        /// <example>.....</example>
        public List<string> Subtypes { get; set; }

        /// <summary>
        /// Supertypes of this card.
        /// </summary>
        /// <example>......</example>
        public List<string> Supertypes { get; set; }

        /// <summary>
        /// Rarity of this card.
        /// </summary>
        /// <example>"Rare"</example>
        public string Rarity { get; set; }

        /// <summary>
        /// Abbriated Set name this card appeared in.
        /// </summary>
        /// <example>"2XM"</example>
        public string Set { get; set; }

        /// <summary>
        /// SetName of this card.
        /// </summary>
        /// <example>"Double Masters"</example>
        public string SetName { get; set; }

        /// <summary>
        /// Functional card Text.
        /// </summary>
        /// <example>"Reveal the top card of your library and put that card into your hand. You lose life equal to its mana value. You may repeat this process any number of times."</example>
        public string Text { get; set; }

        /// <summary>
        /// Flavor text of this card.
        /// </summary>
        /// <example>"When the task spilled over into undeath, he stopped calling it his life's work."</example>
        public string Flavor { get; set; }

        /// <summary>
        /// Artist of this card.
        /// </summary>
        /// <example>"Jeremy Jarvis"</example>
        public string Artist { get; set; }

        /// <summary>
        /// Number of this card within the set it appears in.
        /// </summary>
        /// <example>"76"</example>
        public string Number { get; set; }

        /// <summary>
        /// Power of this card, if its type is creature.
        /// </summary>
        /// <example>"2"</example>
        public string Power { get; set; }

        /// <summary>
        /// Toughness of this card, if its type is creature.
        /// </summary>
        /// <example>"6"</example>
        public string Toughness { get; set; }

        /// <summary>
        /// Layout of this card.
        /// </summary>
        /// <example>"normal"</example>
        public string Layout { get; set; }

        /// <summary>
        /// Watermark of this card.
        /// </summary>
        /// <example>"blah..."</example>
        public string Watermark { get; set; }

        /// <summary>
        /// Date/Text pairs of rulings for this card.
        /// </summary>
        /// <example>["Date:2008-10-01,Text:Each time you put the revealed card into your hand and lose the appropriate amount of life, you decide whether to continue by revealing another card. In other words, you don’t decide in advance how many cards to put into your hand this way."]</example>
        public List<string> Rulings { get; set; }

        /// <summary>
        /// Printings of this card.
        /// </summary>
        /// <example>["2XM", "ALA"]</example>
        public List<string> Printings { get; set; }

        /// <summary>
        /// OriginalText of this card.
        /// </summary>
        /// <example>"Reveal the top card of your library and put that card into your hand. You lose life equal to its converted mana cost. You may repeat this process any number of times."</example>
        public string OriginalText { get; set; }

        /// <summary>
        /// OriginalType of this card.
        /// </summary>
        /// <example>"Instant"</example>
        public string OriginalType { get; set; }

        /// <summary>
        /// Sanctioned formats where this card can be legally played.
        /// </summary>
        /// <example>["Format:Commander,Legality:Legal","Format:Modern,Legality:Legal","Format:Legacy,Legality:Legal","Format:Vintage,Legality:Legal"]</example>
        public List<string> LegalFormats { get; set; }
    }
}
