using Microsoft.AspNetCore.SignalR.Client;
using Quartz;
using Quartz.Impl;
using test;

namespace TestConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("SignalR Console Client");

            var connection = new HubConnectionBuilder().WithUrl("https://localhost:7239/exampleTypeSafeHub").Build();

            await connection.StartAsync();
            Console.WriteLine(connection.State.ToString());

            IScheduler scheduler = await new StdSchedulerFactory().GetScheduler();
            await scheduler.Start();

            string[] baseCurrencies = { "TRY", "EUR" };
            string[] cryptoCurrencies = { "BTC", "SOL", "ETH", "ADA" , "AXS", "STX", "APT", "HOT", "AMP", "BAT" };

            Dictionary<string, string> cryptoImages = new Dictionary<string, string>
            {
                { "BTC", "https://upload.wikimedia.org/wikipedia/commons/thumb/4/46/Bitcoin.svg/1200px-Bitcoin.svg.png" },
                { "SOL", "https://static.vecteezy.com/system/resources/previews/024/093/312/original/solana-sol-glass-crypto-coin-3d-illustration-free-png.png" },
                { "ETH", "https://cloudfront-us-east-1.images.arcpublishing.com/coindesk/ZJZZK5B2ZNF25LYQHMUTBTOMLU.png" },
                { "ADA" , "https://cdn4.iconfinder.com/data/icons/crypto-currency-and-coin-2/256/cardano_ada-512.png"},
                { "AXS" , "https://seeklogo.com/images/A/axie-infinity-axs-logo-57FE26A5DC-seeklogo.com.png" },
                { "STX", "https://cryptologos.cc/logos/stacks-stx-logo.png" },
                { "APT", "https://cryptologos.cc/logos/aptos-apt-logo.png" },
                { "HOT" , "https://s2.coinmarketcap.com/static/img/coins/200x200/2682.png" },
                { "AMP" , "https://cryptologos.cc/logos/amp-amp-logo.png"},
                { "BAT", "https://w7.pngwing.com/pngs/628/626/png-transparent-attention-basic-basicattentiontoken-blockchain-token-blockchain-classic-icon.png" }
            };

            foreach (var baseCurrency in baseCurrencies)
            {
                foreach (var cryptoCurrency in cryptoCurrencies)
                {
                    var jobData = new JobDataMap();
                    jobData.Add("Connection", connection);
                    jobData.Add("BaseCurrency", baseCurrency);
                    jobData.Add("CryptoCurrency", cryptoCurrency);
                    jobData.Add("CryptoImages", cryptoImages);

                    IJobDetail job = JobBuilder.Create<ApiCallJob>()
                        .UsingJobData(jobData)
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity($"trigger_{baseCurrency}_{cryptoCurrency}", "group1")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(20)
                            .RepeatForever())
                        .Build();

                    await scheduler.ScheduleJob(job, trigger);
                }
            }

            Console.WriteLine("Quartz.NET çalışıyor... Çıkmak için bir tuşa basın.");
            Console.ReadLine();

            await scheduler.Shutdown();
        }
    }

    public class ApiCallJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string baseCurrency = context.JobDetail.JobDataMap.GetString("BaseCurrency");
            string cryptoCurrency = context.JobDetail.JobDataMap.GetString("CryptoCurrency");
            var connection = (HubConnection)context.JobDetail.JobDataMap["Connection"];
            var cryptoImages = (Dictionary<string, string>)context.JobDetail.JobDataMap["CryptoImages"];

            var (price, dailyPercent, timestamp) = await PerformApiCall(baseCurrency, cryptoCurrency);

            Console.WriteLine($"Para Birimi: {baseCurrency}");
            Console.WriteLine($"Kripto Para: {cryptoCurrency}");
            Console.WriteLine($"Fiyat: {price}");
            Console.WriteLine($"Günlük Değişim: {dailyPercent}");
            Console.WriteLine($"Zaman: {timestamp:dd/MM/yyyy HH:mm}");

            await connection.InvokeAsync("BroadcastMessageToAllClient", $"{price} {baseCurrency} {cryptoCurrency} {dailyPercent} {cryptoImages[cryptoCurrency]}");

            AddDataToDatabase(baseCurrency, cryptoCurrency, price, timestamp, dailyPercent);
        }

        private async Task<(double, double, DateTime)> PerformApiCall(string baseCurrency, string cryptoCurrency)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.btcturk.com/api/v2/ticker?pairSymbol={cryptoCurrency}{baseCurrency}");
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult>(body);

                var price = resultObject.data[0].last; 
                var dailyPercent = resultObject.data[0].dailyPercent; 
                var timestamp = DateTime.Now; 

                return (price, dailyPercent, timestamp);
            }
        }

        private void AddDataToDatabase(string baseCurrency, string cryptoCurrency, double price, DateTime timestamp, double dailyPercent)
        {
            using (var context = new AppDbContext())
            {
                var cryptoCurrencyData = new CryptoCurrency
                {
                    KriptoPara = cryptoCurrency,
                    ParaCinsi = baseCurrency,
                    Deger = (decimal)price,
                    Tarih = timestamp,
                    Gunluk = (decimal)dailyPercent
                };

                context.CryptoCurrencies.Add(cryptoCurrencyData);
                context.SaveChanges();
            }
        }
    }

    public class ApiResult
    {
        public List<Data> data { get; set; }
    }

    public class Data
    {
        public double last { get; set; }
        public double dailyPercent { get; set; }
    }
}
