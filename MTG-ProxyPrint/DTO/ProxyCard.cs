
namespace ProxyPrint.DTO
{
    public class ProxyCard : SimpleCard
    {
        #region Members.
        public string ImagePath { get; set; }
        #endregion

        #region Constructors.
        public ProxyCard(string imagePath, string name) : base(name)
        {
            ImagePath = imagePath;
        }

        public ProxyCard(string imagePath, string name, int quantity) : base(name, quantity)
        {
            ImagePath = imagePath;
        }
        #endregion
    }
}
