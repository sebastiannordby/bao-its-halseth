using CIS.Application.Legacy;
using CIS.Application.Orders.Models;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Application.Shopify;
using CIS.Library.Orders.Models.Import;
using CIS.Library.Shared.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.EntityFrameworkCore;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CIS.Application.Orders.Services
{
    internal class ImportSalesOrderService : 
        IExecuteImportService<SalesOrderImportDefinition>,
        IMigrateLegacyService<Ordre>,
        IExecuteImportFromShopify<Order>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDbContext;
        private readonly ShopifyClientService _shopifyClientService;

        private Dictionary<int, string> _storeNames = new();
        private Dictionary<int, string> _productNames = new();

        public ImportSalesOrderService(
            CISDbContext dbContext, 
            SWNDistroContext swnDbContext,
            ShopifyClientService shopifyClientService)
        {
            _dbContext = dbContext;
            _swnDbContext = swnDbContext;
            _shopifyClientService = shopifyClientService;
        }

        public async Task ExecuteShopifyImport()
        {
            var latestOrderDate = _dbContext.SalesOrders
                .Max(x => x.OrderDate);

            var getOrdersAfterDateTime = latestOrderDate
                .ToDateTime(TimeOnly.MinValue);

            var shopifyOrders = await _shopifyClientService.GetOrdersAsync(
                getOrdersAfterDateTime);

            var ordersToInsert = new List<SalesOrderDao>();
            var orderLinesToInsert = new List<SalesOrderLineDao>();

            foreach(var shopifyOrder in shopifyOrders)
            {
                var newOrder = new SalesOrderDao()
                {
                    Number = shopifyOrder.OrderNumber ?? 0,
                    AlternateNumber = shopifyOrder.OrderNumber?.ToString(),
                    CustomerName = shopifyOrder.Customer.FirstName,
                    StoreName = shopifyOrder.Customer.LastName,
                    OrderDate = shopifyOrder.CreatedAt.HasValue ? 
                        DateOnly.FromDateTime(shopifyOrder.CreatedAt.Value.DateTime) : DateOnly.FromDateTime(DateTime.Now),
                };
            }
        }

        public async Task<bool> Import(IEnumerable<SalesOrderImportDefinition> definitions)
        {
            try
            {
                var orders = new List<SalesOrderDao>();
                var orderLines = new List<SalesOrderLineDao>();

                foreach (var orderDefinition in definitions)
                {
                    var orderDao = new SalesOrderDao()
                    {
                        Id = Guid.NewGuid(),
                        Number = orderDefinition.Number,
                        AlternateNumber = orderDefinition.AlternateNumber,
                        OrderDate = orderDefinition.OrderDate,
                        Reference = orderDefinition.Reference,
                        DeliveredDate = orderDefinition.DeliveredDate,
                        StoreNumber = orderDefinition.StoreNumber,
                        StoreName = orderDefinition.StoreName,
                        CustomerNumber = orderDefinition.CustomerNumber,
                        CustomerName = orderDefinition.CustomerName,
                        IsDeleted = orderDefinition.IsDeleted
                    };

                    if (string.IsNullOrWhiteSpace(orderDao.StoreName))
                    {
                        orderDao.StoreName = await GetStoreNameCashed(
                            orderDao.StoreNumber) ?? "";
                    }

                    if(string.IsNullOrWhiteSpace(orderDao.CustomerName))
                    {
                        orderDao.CustomerName = orderDao.StoreName;
                    }

                    orders.Add(orderDao);

                    foreach (var lineDefinition in orderDefinition.Lines)
                    {
                        var orderLineDao = new SalesOrderLineDao()
                        {
                            SalesOrderId = orderDao.Id,
                            ProductNumber = lineDefinition.ProductNumber,
                            ProductName = lineDefinition.ProductName,
                            EAN = lineDefinition.EAN,
                            Quantity = lineDefinition.Quantity,
                            QuantityDelivered = lineDefinition.QuantityDelivered,
                            CostPrice = lineDefinition.CostPrice,
                            PurchasePrice = lineDefinition.PurchasePrice,
                            StorePrice = lineDefinition.StorePrice,
                            CurrencyCode = "NOK"
                        };

                        if (string.IsNullOrWhiteSpace(orderLineDao.ProductName))
                        {
                            orderLineDao.ProductName = await GetProductNameCached(
                                orderLineDao.ProductNumber, orderLineDao.EAN);
                        }

                        orderLines.Add(orderLineDao);
                    }

                }

                await _dbContext.SalesOrders.AddRangeAsync(orders);
                await _dbContext.SalesOrderLines.AddRangeAsync(orderLines);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task Migrate(Func<string, Task> log)
        {
            await log("Importering av ordre/bestillinger påbegynt.");

            var allOrderGroupingsQuery = _swnDbContext.Ordres
                .AsNoTracking()
                .GroupBy(x => new { x.Dato, x.Butikknr })
                .Select(x => new
                {
                    Dato = x.Key.Dato,
                    Butikknr = x.Key.Butikknr
                });

            await DbSetExtensions.ProcessEntitiesInBatches(allOrderGroupingsQuery, async (orderGroupings, percentageDone) =>
            {
                var importDefinitions = new List<SalesOrderImportDefinition>();

                foreach (var test in orderGroupings)
                {
                    var grouping = await _swnDbContext.Ordres
                        .AsNoTracking()
                        .Where(x => x.Butikknr == test.Butikknr)
                        .Where(x => x.Dato == test.Dato)
                        .ToListAsync();

                    var legOrder = grouping.First();
                    var importOrder = new SalesOrderImportDefinition()
                    {
                        Number = (int)legOrder.Id,
                        AlternateNumber = legOrder.NettOrdreRef,
                        StoreNumber = legOrder.Butikknr ?? 0,
                        StoreName = null,
                        CustomerNumber = legOrder.Butikknr ?? 0,
                        CustomerName = null,
                        DeliveredDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        Reference = legOrder.Ordreref as string,
                        IsDeleted = (legOrder.Ordretype ?? "").Equals("Slettet", StringComparison.OrdinalIgnoreCase)
                    };

                    foreach (var orderLine in grouping)
                    {
                        var importLine = new SalesOrderImportDefinition.Line()
                        {
                            CostPrice = orderLine.OurPrice,
                            EAN = orderLine.Ean,
                            ProductName = null,
                            Quantity = orderLine.Antall ?? 0,
                            PurchasePrice = orderLine.Innpris,
                            QuantityDelivered = orderLine.AntallLevert ?? 0,
                            CurrencyCode = "NOK"
                        };

                        importOrder.Lines.Add(importLine);
                    }

                    importDefinitions.Add(importOrder);
                }

                var success = await Import(importDefinitions);

                var textLines = importDefinitions
                    .Select(x => x.Number.ToString())
                    .ToArray();

                var text = string.Join("\n", textLines);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentageDone}%)({successMsg}) Ordre:\n{text}\n";
                await log(message);
            });

            await log("Importering av ordre/bestillinger vellykket.");
        }

        private async Task<string> GetProductNameCached(
            int productNumber, string ean)
        {
            if (_productNames.ContainsKey(productNumber))
                return _productNames[productNumber];

            var productName = await _dbContext.Products
                .Where(x =>
                    x.Number == productNumber ||
                    x.EAN == ean)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
            _productNames.Add(productNumber, productName);

            return productName;
        }

        private async Task<string?> GetStoreNameCashed(int storeNumber)
        {
            if (storeNumber == 0)
                return null;

            if (_storeNames.ContainsKey(storeNumber))
                return _storeNames[storeNumber];

            var storeName = await _dbContext.Products
                .Where(x =>
                    x.Number == storeNumber)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? string.Empty;

            _storeNames.Add(storeNumber, storeName);

            return storeName;
        }
    }
}
