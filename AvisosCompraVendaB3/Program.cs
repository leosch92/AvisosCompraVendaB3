using System;
using System.Globalization;
using System.Threading.Tasks;
using AvisosCompraVendaB3.Model;
using Quartz.Impl;
using Quartz;
using AvisosCompraVendaB3.Job;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AvisosCompraVendaB3
{
    class Program
    {
        static async Task Main(string[] args)
        {

            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            var configuration = GetConfiguration();

            AvisosContext context;
            try
            {
                context = MakeContext(args);
            }
            catch (FormatException)
            {
                Console.WriteLine("Formato de preço incorreto. Favor passar um preço com ponto como separador de decimais.");
                return;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Número de argumentos passados incorreto.");
                return;
            }

            await ScheduleJob(context, configuration);
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            return configuration;
        }

        private static AvisosContext MakeContext(string[] args)
        {
            var assetName = args[0];
            var sellPrice = Decimal.Parse(args[1]);
            var buyPrice = Decimal.Parse(args[2]);
            var context = new AvisosContext(assetName, buyPrice, sellPrice);
            return context;
        }

        private static async Task ScheduleJob(AvisosContext context, IConfigurationRoot configuration)
        {
            JobDataMap dataMap = new JobDataMap();
            dataMap.Put("context", context);
            dataMap.Put("configuration", configuration);

            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<AvisosCompraVendaJob>()
                .WithIdentity("avisos-job", "avisos-group")
                .UsingJobData(dataMap)
                .Build();

            var jobInterval = Int32.Parse(configuration["emailJobIntervalInSeconds"]);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("avisos-trigger", "avisos-group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(jobInterval)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            await Task.Delay(TimeSpan.FromDays(1));
            await scheduler.Shutdown();
        }
    }
}
