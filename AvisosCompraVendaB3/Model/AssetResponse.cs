using System.Collections.Generic;

namespace AvisosCompraVendaB3.Model
{
    public class AssetResponse
    {
        public string By { get; set; }
        public bool Valid_key { get; set; }
        public Dictionary<string, Asset> Results { get; set; }
        public double Execution_time { get; set; }
        public bool From_cache { get; set; }
    }
}
