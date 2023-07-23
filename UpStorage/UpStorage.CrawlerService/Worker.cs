using Microsoft.AspNetCore.SignalR.Client;
using UpStorage.Domain.Dtos;
using UpStorage.Domain.Enum;

namespace UpStorage.CrawlerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HubConnection _hubConnection;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:7275/Hubs/UpStorageOrderHub")
                .WithAutomaticReconnect()
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _hubConnection.On<UpStorageAddOrderDto>("AddedOrder", async (order) =>
            {
                //_logger.LogInformation{ "New order added:",order};
                Console.WriteLine(order.RequestedAmount);
                Console.WriteLine(order.ProductCrawlType);


                Crawler crawler = new Crawler();
                await crawler.StartCrawling(order.RequestedAmount, order.ProductCrawlType);

            });

            await _hubConnection.StartAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
            //    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    //await Task.Delay(1000, stoppingToken);
            }
        }
    }
}