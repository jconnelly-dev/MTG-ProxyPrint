
namespace ProxyPrint.DTO
{
    public class SimpleCard
    {
        #region Members.
        public string Name { get; set; }
        public int Quantity { get; set; }
        #endregion

        #region Constructors.
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
        #endregion
    }
}
