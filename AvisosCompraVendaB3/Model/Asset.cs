namespace AvisosCompraVendaB3.Model
{
    public class Asset
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Company_name { get; set; }
        public string Document { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Region { get; set; }
        public string Currency { get; set; }
        public MarketTime Market_time { get; set; }
        public int Market_cap { get; set; }
        public decimal Price { get; set; }
        public double Change_percent { get; set; }
        public string Updated_at { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
