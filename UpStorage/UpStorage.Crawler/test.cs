//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR.Client;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using UpStorage.Crawler.GetUser;
//using UpStorage.Domain.Dtos;
//using UpStorage.Domain.Entities;
//using UpStorage.Domain.Enum;

//public class Crawler
//{
//    private IWebDriver driver;
//    private readonly string logHubUrl = "https://localhost:7275/Hubs/UpStorageLogHub";
//    private readonly string orderHubUrl = "https://localhost:7275/Hubs/UpStorageOrderHub";
//    private readonly string orderEventHubUrl = "https://localhost:7275/Hubs/UpStorageOrderEventHub";
//    private readonly string productHubUrl = "https://localhost:7275/Hubs/UpStorageProductHub";

//    private HubConnection logHubConnection;
//    private HubConnection orderHubConnection;
//    private HubConnection orderEventHubConnection;
//    private HubConnection productHubConnection;

//    private List<Product> allProductList;

//    public async Task StartCrawling(int requestedAmount, ProductCrawlType crawlType)
//    {
//        try
//        {
//            InitializeDriver();

//            await InitializeHubConnections();

//            allProductList = new List<Product>();

//            var order = new Order
//            {
//                Id = Guid.NewGuid(),
//                CreatedOn = DateTimeOffset.Now,
//                RequestedAmount = requestedAmount,
//                TotalFoundAmount = allProductList.Count,
//                ProductCrawlType = crawlType
//            };

//            var orderId = order.Id;

//            await AddOrderEventAsync(orderId, OrderStatus.BotStarted);

//            await SendLogNotificationAsync("Crawling started.");

//            driver.Navigate().GoToUrl("https://4teker.net/");

//            var pageCount = driver.FindElements(By.XPath("/html/body/section/div/nav/ul/li"));
//            int pageCounter = 1;

//            await AddOrderAsync(order.Id, order.CreatedOn, order.RequestedAmount, order.TotalFoundAmount, order.ProductCrawlType);

//            await AddOrderEventAsync(orderId, OrderStatus.CrawlingStarted);

//            while (pageCounter <= pageCount.Count - 1)
//            {
//                await SendLogNotificationAsync($"Page number to crawl: {pageCounter}");
//                if (pageCounter == 1)
//                {
//                    driver.Navigate().GoToUrl("https://4teker.net/");
//                }
//                else
//                {
//                    driver.Navigate().GoToUrl($"https://4teker.net/?currentPage={pageCounter}");
//                }

//                // Crawl products on the page
//                // ...

//                pageCounter++;
//            }

//            await SendLogNotificationAsync("Crawling completed.");
//            await AddOrderEventAsync(orderId, OrderStatus.CrawlingCompleted);

//            List<Product> filteredProducts = new List<Product>();
//            switch (crawlType)
//            {
//                case ProductCrawlType.All:
//                    filteredProducts = allProductList;
//                    Console.WriteLine("Tüm ürünler listelenmiştir.");
//                    break;

//                case ProductCrawlType.OnDiscount:
//                    filteredProducts = allProductList.Where(p => p.IsOnSale).ToList();
//                    Console.WriteLine("İndirimli ürünler listelenmiştir.");
//                    break;

//                case ProductCrawlType.NonDiscount:
//                    filteredProducts = allProductList.Where(p => !p.IsOnSale).ToList();
//                    Console.WriteLine("İndirimli ürünler listelenmiştir.");
//                    break;

//                default:
//                    throw new Exception("Geçersiz ürün tipi seçilmiştir.");
//            }

//            ExcelProcess excelProcess = new ExcelProcess();
//            excelProcess.WriteAndSendList(filteredProducts);

//            await SendLogNotificationAsync("A list of products was prepared and sent by e-mail.");
//            await AddOrderEventAsync(orderId, OrderStatus.OrderCompleted);
//            await SendLogNotificationAsync("Order Completed.");
//        }
//        catch (Exception exception)
//        {
//            await SendLogNotificationAsync(exception.Message.ToString());

//            var orderId = allProductList.FirstOrDefault()?.OrderId ?? Guid.Empty;
//            await AddOrderEventAsync(orderId, OrderStatus.CrawlingFailed);

//            driver.Quit();
//        }
//    }

//    private void InitializeDriver()
//    {
//        driver = new ChromeDriver();
//    }

//    private async Task InitializeHubConnections()
//    {
//        logHubConnection = new HubConnectionBuilder()
//            .WithUrl(logHubUrl)
//            .WithAutomaticReconnect()
//            .Build();

//        await logHubConnection.StartAsync();

//        orderHubConnection = new HubConnectionBuilder()
//            .WithUrl(orderHubUrl)
//            .WithAutomaticReconnect()
//            .Build();

//        await orderHubConnection.StartAsync();

//        orderEventHubConnection = new HubConnectionBuilder()
//            .WithUrl(orderEventHubUrl)
//            .WithAutomaticReconnect()
//            .Build();

//        await orderEventHubConnection.StartAsync();

//        productHubConnection = new HubConnectionBuilder()
//            .WithUrl(productHubUrl)
//            .WithAutomaticReconnect()
//            .Build();

//        await productHubConnection.StartAsync();
//    }

//    private async Task AddOrderEventAsync(Guid orderId, OrderStatus status)
//    {
//        var orderEvent = new OrderEvent
//        {
//            OrderId = orderId,
//            Status = status
//        };

//        await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));
//    }

//    private async Task SendLogNotificationAsync(string message)
//    {
//        await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog(message));
//    }

//    private async Task AddOrderAsync(Guid id, DateTimeOffset createdOn, int requestedAmount, int totalFoundAmount, ProductCrawlType productCrawlType)
//    {
//        await orderHubConnection.InvokeAsync("AddOrderAsync", AddedOrder(id, createdOn, requestedAmount, totalFoundAmount, productCrawlType));
//    }

//    private UpStorageLogDto CreateLog(string message) => new UpStorageLogDto(message);

//    private UpStorageOrderEventDto AddedOrderEvent(Guid orderId, OrderStatus status) => new UpStorageOrderEventDto(orderId, status);

//    private UpStorageProductDto AddedProduct(Guid id, Guid orderId, DateTimeOffset createdOn, string name, string picture, bool isOnSale, decimal price, decimal salePrice) =>
//        new UpStorageProductDto(id, orderId, createdOn, name, picture, isOnSale, price, salePrice);

//    private UpStorageOrderDto AddedOrder(Guid id, DateTimeOffset createdOn, int requestedAmount, int totalFoundAmount, ProductCrawlType productCrawlType) =>
//        new UpStorageOrderDto(id, createdOn, requestedAmount, totalFoundAmount, productCrawlType);
//}
