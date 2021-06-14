
namespace MagicCardAPI.API_DTO
{
    public class MagicCardDTO
    {
        public string name;
        public string manaCost;
        public string cmc;
        public string[] colors;
        public string[] colorIdentity;
        public string type;
        public string[] supertypes;
        public string[] types;
        public string[] subtypes;
        public string rarity;
        public string set;
        public string setName;
        public string text;
        public string artist;
        public string number;
        public string power;
        public string toughness;
        public string layout;
        public int multiverseid;
        public string imageUrl;
        public string watermark;
        public MagicRulingDTO[] rulings;
        public MagicForeignNamesDTO[] foreignNames;
        public string[] printings;
        public string originalText;
        public string originalType;
        public MagicLegalityDTO[] legalities;
        public string id;
    }
}
