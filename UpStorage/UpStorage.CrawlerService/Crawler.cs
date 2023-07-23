using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using UpStorage.Domain.Dtos;
using UpStorage.Domain.Entities;
using UpStorage.Domain.Enum;

namespace UpStorage.CrawlerService
{
    public class Crawler
    {
        private IWebDriver driver;
        private readonly string logHubUrl = "https://localhost:7275/Hubs/UpStorageLogHub";
        private readonly string orderHubUrl = "https://localhost:7275/Hubs/UpStorageOrderHub";
        private readonly string orderEventHubUrl = "https://localhost:7275/Hubs/UpStorageOrderEventHub";
        private readonly string productHubUrl = "https://localhost:7275/Hubs/UpStorageProductHub";

        private HubConnection logHubConnection;
        private HubConnection orderHubConnection;
        private HubConnection orderEventHubConnection;
        private HubConnection productHubConnection;
        List<Product> allProductList = new List<Product>();
        public async Task StartCrawling(int requestedAmount, string crawlType)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTimeOffset.Now,
                RequestedAmount = requestedAmount,
                TotalFoundAmount = allProductList.Count,
                ProductCrawlType = crawlType
            };

            var orderId = order.Id;

            var orderEvent = new OrderEvent()
            {
                OrderId = orderId,
                Status = OrderStatus.CrawlingStarted
            };

            try
            {
                InitializeDriver();

                await InitializeHubConnections();
                

                //await AddOrderEventAsync(orderId, OrderStatus.BotStarted);

                await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Crawling started."));
                Console.WriteLine("Crawling started.");

                driver.Navigate().GoToUrl("https://4teker.net/");

                var pageCount = driver.FindElements(By.XPath("/html/body/section/div/nav/ul/li"));
                int pageCounter = 1;

                await AddOrderAsync(order.Id, order.CreatedOn, order.RequestedAmount, order.TotalFoundAmount, order.ProductCrawlType);

                orderEvent = new OrderEvent()
                {
                    OrderId = orderId,
                    Status = OrderStatus.CrawlingStarted
                };

                await AddOrderEventAsync(orderEvent.OrderId, orderEvent.Status);

                while (pageCounter <= pageCount.Count - 1)
                {
                    await SendLogNotificationAsync($"Page number to crawl: {pageCounter}");
                    if (pageCounter == 1)
                    {
                        driver.Navigate().GoToUrl("https://4teker.net/");
                    }
                    else
                    {
                        driver.Navigate().GoToUrl($"https://4teker.net/?currentPage={pageCounter}");
                    }

                    bool isOnSale = false;
                    var PageProduct = driver.FindElements(By.XPath("/html/body/section/div/div/div"));
                    int pageProductCounter = 1;

                    decimal price = 0;
                    decimal salePrice = 0;


                    foreach (var Product in PageProduct)
                    {

                        //for price value
                        try
                        {
                            if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[1]")).GetAttribute("class").Contains("text-muted text-decoration-line-through price"))
                            {

                                price = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[1]")).Text.Replace("$", "").Replace(",", "."));
                            }
                        }
                        catch
                        {
                            if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span")).GetAttribute("class").Contains("price"))
                            {
                                price = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span[1]")).Text.Replace("$", "").Replace(",", "."));
                            }
                        }

                        //for sale price value
                        try
                        {
                            if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[2]")).GetAttribute("class").Contains("sale-price"))
                            {
                                salePrice = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[2]")).Text.Replace("$", "").Replace(",", "."));
                            }
                        }
                        catch
                        {
                            if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span")).GetAttribute("class").Contains("price"))
                            {
                                salePrice = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span")).Text.Replace("$", "").Replace(",", "."));
                            }
                        }

                        //for is on sale
                        if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]")).Text.Contains("sale"))
                        {
                            isOnSale = true;
                        }
                        else
                        {
                            isOnSale = false;
                        }

                        var extractproduct = new Product()
                        {
                            OrderId = orderId,
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTimeOffset.Now,
                            Name = driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[*]/div/h5")).Text,
                            Picture = driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/img")).GetAttribute("src"),
                            IsOnSale = isOnSale,
                            Price = price,
                            SalePrice = salePrice,
                        };

                        allProductList.Add(extractproduct);

                        await productHubConnection.InvokeAsync("AddProductAsync", AddedProduct(extractproduct.Id, extractproduct.OrderId, extractproduct.CreatedOn,
                            extractproduct.Name, extractproduct.Picture, extractproduct.IsOnSale, extractproduct.Price, extractproduct.SalePrice));

                        pageProductCounter++;

                    }
                    await SendLogNotificationAsync($"Page {pageCounter} crawling is complete.");
                    pageCounter++;
                }


                await AddOrderEventAsync(orderId, OrderStatus.CrawlingCompleted);
                await SendLogNotificationAsync("Crawling completed.");
            }
            catch (Exception exception)
            {
                await SendLogNotificationAsync(exception.Message.ToString());

                await AddOrderEventAsync(orderId, OrderStatus.CrawlingFailed);

                driver.Quit();
            }

            List<Product> filteredProducts = new List<Product>();
            switch (crawlType)
            {
                case "All":
                    filteredProducts = allProductList;
                    Console.WriteLine("Tüm ürünler listelenmiştir.");
                    break;

                case "OnDiscount":
                    filteredProducts = allProductList.Where(p => p.IsOnSale).ToList();
                    Console.WriteLine("İndirimli ürünler listelenmiştir.");
                    break;

                case "NonDiscount":
                    filteredProducts = allProductList.Where(p => !p.IsOnSale).ToList();
                    Console.WriteLine("İndirimli ürünler listelenmiştir.");
                    break;

                default:
                    throw new Exception("Geçersiz ürün tipi seçilmiştir.");
            }

            orderEvent = new OrderEvent()
            {
                OrderId = orderId,
                Status = OrderStatus.OrderCompleted,
            };
            await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));

            await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Order Completed."));
        }

        private void InitializeDriver()
        {
            driver = new ChromeDriver();
        }

        private async Task InitializeHubConnections()
        {
            logHubConnection = new HubConnectionBuilder()
                .WithUrl(logHubUrl)
                .WithAutomaticReconnect()
                .Build();

            await logHubConnection.StartAsync();

            orderHubConnection = new HubConnectionBuilder()
                .WithUrl(orderHubUrl)
                .WithAutomaticReconnect()
                .Build();

            await orderHubConnection.StartAsync();

            orderEventHubConnection = new HubConnectionBuilder()
                .WithUrl(orderEventHubUrl)
                .WithAutomaticReconnect()
                .Build();

            await orderEventHubConnection.StartAsync();

            productHubConnection = new HubConnectionBuilder()
                .WithUrl(productHubUrl)
                .WithAutomaticReconnect()
                .Build();

            await productHubConnection.StartAsync();
        }

        private async Task AddOrderEventAsync(Guid orderId, OrderStatus status)
        {
            var orderEvent = new OrderEvent
            {
                OrderId = orderId,
                Status = status
            };

            await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));
        }

        private async Task SendLogNotificationAsync(string message)
        {
            await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog(message));
        }

        private async Task AddOrderAsync(Guid id, DateTimeOffset createdOn, int requestedAmount, int totalFoundAmount, string productCrawlType)
        {
            await orderHubConnection.InvokeAsync("AddOrderAsync", AddedOrder(id, createdOn, requestedAmount, totalFoundAmount, productCrawlType));
        }

        public UpStorageAddLogDto CreateLog(string message) => new UpStorageAddLogDto(message);

        private UpStorageOrderEventDto AddedOrderEvent(Guid orderId, OrderStatus status) => new UpStorageOrderEventDto(orderId, status);

        private UpStorageProductDto AddedProduct(Guid id, Guid orderId, DateTimeOffset createdOn, string name, string picture, bool isOnSale, decimal price, decimal salePrice) =>
            new UpStorageProductDto(id, orderId, createdOn, name, picture, isOnSale, price, salePrice);

        private UpStorageOrderDto AddedOrder(Guid id, DateTimeOffset createdOn, int requestedAmount, int totalFoundAmount, string productCrawlType) =>
            new UpStorageOrderDto(id, createdOn, requestedAmount, totalFoundAmount, productCrawlType);
    }
}