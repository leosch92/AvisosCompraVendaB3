using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using RestSharp;
using AvisosCompraVendaB3.Model;
using System.Net.Mail;

namespace AvisosCompraVendaB3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            string apiKey = configuration["apiKey"];
            string targetEmail = configuration["targetEmail"];

            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

            try
            {
                var assetName = args[0];
                var buyPrice = Decimal.Parse(args[1]);
                var sellPrice = Decimal.Parse(args[2]);

                var client = new RestClient($"https://api.hgbrasil.com/");
                var request = new RestRequest($"finance/stock_price?key={apiKey}&symbol={assetName}", DataFormat.Json);
                var response = client.Get<AssetResponse>(request);

                var asset = response.Data.Results[assetName];

                var smtpConfig = configuration.GetSection("smtpConfig").Get<SmtpConfig>();

                MailMessage message = new MailMessage(smtpConfig.Sender, targetEmail);
                message.Subject = "Aconselhamento de Venda";
                message.Body = $"O ativo {assetName} está com um preço de R${asset.Price}. É aconselhável que você venda esse ativo.";
                SmtpClient smtpClient = new SmtpClient(smtpConfig.Server, smtpConfig.Port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpConfig.Username, smtpConfig.Password);

                try
                {
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                        ex.ToString());
                }


            }
            catch (IndexOutOfRangeException ie)
            {
                Console.WriteLine("Número de argumentos passados incorreto.");
                throw;
            }

        }
    }
}
