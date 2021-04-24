using AvisosCompraVendaB3.Model;
using Microsoft.Extensions.Configuration;
using Quartz;
using RestSharp;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AvisosCompraVendaB3.Job
{
    public class AvisosCompraVendaJob: IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var avisosContext = GetAvisosContext(context);
            var configuration = GetConfiguration(context);
            var asset = await GetAsset(avisosContext, configuration);
            TrySendEmailOnValidAssetResponse(avisosContext, configuration, asset);
        }

        private static void TrySendEmailOnValidAssetResponse(AvisosContext avisosContext, IConfigurationRoot configuration, Asset asset)
        {
            if (asset.Error)
            {
                Console.WriteLine(asset.Message);
            }
            else
            {
                TryToSendEmail(avisosContext, configuration, asset);
            }
        }

        private static void TryToSendEmail(AvisosContext avisosContext, IConfigurationRoot configuration, Asset asset)
        {
            if (HasToSendEMail(avisosContext, asset))
            {
                AvisoMailData mailData = MakeEmailData(avisosContext, configuration, asset);
                MakeAndSendEmail(mailData);
            }
        }

        private static AvisosContext GetAvisosContext(IJobExecutionContext context)
        {
            return (AvisosContext)context.JobDetail.JobDataMap.Get("context");
        }

        private static IConfigurationRoot GetConfiguration(IJobExecutionContext context)
        {
            return (IConfigurationRoot) context.JobDetail.JobDataMap.Get("configuration");
        }

        private static bool HasToSendEMail(AvisosContext avisosContext, Asset asset)
        {
            return asset.Price <= avisosContext.BuyPrice || asset.Price >= avisosContext.SellPrice;
        }

        private static async Task<Asset> GetAsset(AvisosContext avisosContext, IConfigurationRoot configuration)
        {
            try
            {
                string apiKey = configuration["apiKey"];
                var client = new RestClient($"https://api.hgbrasil.com/");
                var request = new RestRequest($"finance/stock_price?key={apiKey}&symbol={avisosContext.AssetName}", DataFormat.Json);
                Console.WriteLine($"Pegando informações do ativo {avisosContext.AssetName}...");
                var response = await client.GetAsync<AssetResponse>(request);
                var asset = response.Results[avisosContext.AssetName.ToUpper()];
                return asset;
            }
            catch (Exception)
            {
                Console.WriteLine("Erro ao fazer requisição para API de cotações.");
                throw;
            }
        }

        private static AvisoMailData MakeEmailData(AvisosContext avisosContext, IConfigurationRoot configuration, Asset asset)
        {
            var smtpConfig = configuration.GetSection("smtpConfig").Get<SmtpConfig>();
            string targetEmail = configuration["targetEmail"];
            var mailData = new AvisoMailData(avisosContext, asset, targetEmail, smtpConfig);
            return mailData;
        }

        private static void MakeAndSendEmail(AvisoMailData data)
        {
            MailMessage message = MakeMessage(data);
            Console.WriteLine($"Enviando e-mail com aconselhamento para {data.TargetEmail}...");
            SendEmail(data, message);
        }

        private static MailMessage MakeMessage(AvisoMailData data)
        {
            MailMessage message = new MailMessage(data.SmtpConfig.Sender, data.TargetEmail);
            if (data.Asset.Price < data.BuyPrice)
            {
                SetBuyMessage(data, message);
            }
            else if (data.Asset.Price > data.SellPrice)
            {
                SetSellMessage(data, message);
            }

            return message;
        }

        private static void SetBuyMessage(AvisoMailData data, MailMessage message)
        {
            message.Subject = "Aconselhamento de Compra de Ativo";
            message.Body = $"O ativo {data.AssetName} está com um preço de R${data.Asset.Price}. Você definiu que um bom preço para compra desse ativo é de R${data.BuyPrice}, portanto é aconselhável que você o compre.";
        }

        private static void SetSellMessage(AvisoMailData data, MailMessage message)
        {
            message.Subject = "Aconselhamento de Venda de Ativo";
            message.Body = $"O ativo {data.AssetName} está com um preço de R${data.Asset.Price}. Você definiu que um bom preço para venda desse ativo é de R${data.SellPrice}, portanto é aconselhável que você o venda.";
        }

        private static void SendEmail(AvisoMailData data, MailMessage message)
        {
            SmtpClient smtpClient = new SmtpClient(data.SmtpConfig.Server, data.SmtpConfig.Port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(data.SmtpConfig.Username, data.SmtpConfig.Password);

            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar e-mail.",ex.ToString());
            }
        }
    }
}
