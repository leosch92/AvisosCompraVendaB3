namespace AvisosCompraVendaB3.Model
{
    public class AvisosContext
    {
        public string AssetName { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }

        public AvisosContext(string assetName, decimal buyPrice, decimal sellPrice)
        {
            AssetName = assetName;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }
    }
}
