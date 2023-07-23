﻿//using Microsoft.AspNetCore.SignalR.Client;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using UpStorage.Crawler.GetUser;
//using UpStorage.Domain.Dtos;
//using UpStorage.Domain.Entities;
//using UpStorage.Domain.Enum;

//GetUserPreferences getUserPreferences = new GetUserPreferences();

//getUserPreferences.Greetings();
//getUserPreferences.ReadInputs();

//ProductCrawlType crawlType = getUserPreferences.productCrawlType;

int requestedAmount = 0;

//Console.ReadKey();
//Thread.Sleep(1000);

//IWebDriver driver = new ChromeDriver();

//var logHubConnection = new HubConnectionBuilder()
//   .WithUrl("https://localhost:7275/Hubs/UpStorageLogHub")
//   .WithAutomaticReconnect()
//   .Build();

//await logHubConnection.StartAsync();

//var orderHubConnection = new HubConnectionBuilder()
//   .WithUrl("https://localhost:7275/Hubs/UpStorageOrderHub")
//   .WithAutomaticReconnect()
//   .Build();

//await orderHubConnection.StartAsync();

//var orderEventHubConnection = new HubConnectionBuilder()
//   .WithUrl("https://localhost:7275/Hubs/UpStorageOrderEventHub")
//   .WithAutomaticReconnect()
//   .Build();

//await orderEventHubConnection.StartAsync();

//var productHubConnection = new HubConnectionBuilder()
//   .WithUrl("https://localhost:7275/Hubs/UpStorageProductHub")
//   .WithAutomaticReconnect()
//   .Build();

//await productHubConnection.StartAsync();

//List<Product> allProductList = new List<Product>();

//var order = new Order()
//{
//    Id = Guid.NewGuid(),
//    CreatedOn = DateTimeOffset.Now,
//    RequestedAmount = requestedAmount,
//    TotalFoundAmount = allProductList.Count,
//    ProductCrawlType = crawlType
//};
//var orderId = order.Id;

//var orderEvent = new OrderEvent()
//{
//    OrderId = orderId,
//    Status = OrderStatus.BotStarted
//};

//await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));

//try
//{
//    await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Crawling started."));

//    driver.Navigate().GoToUrl("https://4teker.net/");

//    var pageCount = driver.FindElements(By.XPath("/html/body/section/div/nav/ul/li"));

//    int pageCounter = 1;

//    await orderHubConnection.InvokeAsync("AddOrderAsync", AddedOrder(order.Id, order.CreatedOn, order.RequestedAmount,
//        order.TotalFoundAmount, order.ProductCrawlType));

//    orderEvent = new OrderEvent()
//    {
//        OrderId = orderId,
//        Status = OrderStatus.CrawlingStarted
////    };

//    await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));


//    while (pageCounter <= pageCount.Count - 1)
//    {
//        await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"Page number to crawl : {pageCounter}"));
//        if (pageCounter == 1)
//        {
//            driver.Navigate().GoToUrl("https://4teker.net/");
//        }
//        else
//        {
//            driver.Navigate().GoToUrl($"https://4teker.net/?currentPage={pageCounter}");
//        }

//        bool isOnSale = false;
//        var PageProduct = driver.FindElements(By.XPath("/html/body/section/div/div/div"));  //Bir sayfadaki ürünler için konuldu.
//        int pageProductCounter = 1;

//        decimal price = 0;
//        decimal salePrice = 0;

//        foreach (var Product in PageProduct)
//        {

//            //For price value
//            try
//            {
//                if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[1]")).GetAttribute("class").Contains("text-muted text-decoration-line-through price"))
//                {

//                    price = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[1]")).Text.Replace("$", "").Replace(",", "."));
//                }
//            }
//            catch
//            {
//                if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span")).GetAttribute("class").Contains("price"))
//                {
//                    price = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span[1]")).Text.Replace("$", "").Replace(",", "."));
//                }
//            }

//            //For sale price value
//            try
//            {
//                if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[2]")).GetAttribute("class").Contains("sale-price"))
//                {
//                    salePrice = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[2]/div/span[2]")).Text.Replace("$", "").Replace(",", "."));
//                }
//            }
//            catch
//            {
//                if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span")).GetAttribute("class").Contains("price"))
//                {
//                    salePrice = Convert.ToDecimal(driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]/div/span")).Text.Replace("$", "").Replace(",", "."));
//                }
//            }

//            //For is on sale
//            if (driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[1]")).Text.Contains("Sale"))
//            {
//                isOnSale = true;
//            }
//            else
//            {
//                isOnSale = false;
//            }

//            var extractProduct = new Product()
//            {
//                OrderId = orderId,
//                Id = Guid.NewGuid(),
//                CreatedOn = DateTimeOffset.Now,
//                Name = driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/div[*]/div/h5")).Text,
//                Picture = driver.FindElement(By.XPath($"/html/body/section/div/div/div[{pageProductCounter}]/div/img")).GetAttribute("src"),
//                IsOnSale = isOnSale,
//                Price = price,
//                SalePrice = salePrice,
//            };

//            allProductList.Add(extractProduct);

//            await productHubConnection.InvokeAsync("AddProductAsync", AddedProduct(extractProduct.Id, extractProduct.OrderId, extractProduct.CreatedOn,
//                extractProduct.Name, extractProduct.Picture, extractProduct.IsOnSale, extractProduct.Price, extractProduct.SalePrice));

//            pageProductCounter++;
//        }
//        await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"Page {pageCounter} crawling is complete."));
//        pageCounter++;
//    }
//    orderEvent = new OrderEvent()
//    {
//        OrderId = orderId,
//        Status = OrderStatus.CrawlingCompleted
//    };
//    await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Crawling completed."));
//    await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));
//    //Console.ReadLine();
//}
//catch (Exception exception)
//{
//    await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog(exception.Message.ToString()));

//    orderEvent = new OrderEvent()
//    {
//        OrderId = orderId,
//        Status = OrderStatus.CrawlingFailed
//    };

//    await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));

//    driver.Quit();
//}
//List<Product> filteredProducts = new List<Product>();
//switch (crawlType)
//{
//    case ProductCrawlType.All:
//        filteredProducts = allProductList;
//        Console.WriteLine("Tüm ürünler listelenmiştir.");
//        break;

//    case ProductCrawlType.OnDiscount:
//        filteredProducts = allProductList.Where(p => p.IsOnSale).ToList();
//        Console.WriteLine("İndirimli ürünler listelenmiştir.");
//        break;

//    case ProductCrawlType.NonDiscount:
//        filteredProducts = allProductList.Where(p => p.IsOnSale).ToList();
//        Console.WriteLine("İndirimli ürünler listelenmiştir.");
//        break;

//    default:
//        new Exception("Geçersiz ürün tipi seçilmiştir.");
//        break;
//}

//ExcelProcess excelProcess = new ExcelProcess();
//excelProcess.WriteAndSendList(filteredProducts);
//await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("A list of products was prepared and sent by e-mail."));

//orderEvent = new OrderEvent()
//{
//    OrderId = orderId,
//    Status = OrderStatus.OrderCompleted
//};

//await orderEventHubConnection.InvokeAsync("AddOrderEventAsync", AddedOrderEvent(orderEvent.OrderId, orderEvent.Status));

//await logHubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Order Completed."));
//UpStorageLogDto CreateLog(string message) => new UpStorageLogDto(message);

//UpStorageOrderEventDto AddedOrderEvent(Guid orderId, OrderStatus status) => new UpStorageOrderEventDto(orderId, status);

//UpStorageProductDto AddedProduct(Guid id, Guid orderId, DateTimeOffset createdOn, string name, string picture, bool isOnSale, decimal price, decimal salePrice) =>
//    new UpStorageProductDto(id, orderId, createdOn, name, picture, isOnSale, price, salePrice);

//UpStorageOrderDto AddedOrder(Guid id, DateTimeOffset createdOn, int requestedAmount, int totalFoundAmount, ProductCrawlType productCrawlType) =>
//    new UpStorageOrderDto(id, createdOn, requestedAmount, totalFoundAmount, productCrawlType);



