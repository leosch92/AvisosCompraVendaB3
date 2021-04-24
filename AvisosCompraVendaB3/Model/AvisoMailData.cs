using System;
using System.Collections.Generic;
using System.Text;

namespace AvisosCompraVendaB3.Model
{
    public class AvisoMailData
    {
        public string AssetName { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public Asset Asset { get; set; }
        public string TargetEmail { get; set; }
        public SmtpConfig SmtpConfig { get; set; }

        public AvisoMailData(AvisosContext context, Asset asset, string targetEmail, SmtpConfig smtpConfig)
        {
            AssetName = context.AssetName;
            BuyPrice = context.BuyPrice;
            SellPrice = context.SellPrice;
            Asset = asset;
            TargetEmail = targetEmail;
            SmtpConfig = smtpConfig;
        }
    }
}
