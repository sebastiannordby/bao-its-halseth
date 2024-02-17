using CIS.Application.Legacy;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Application.Shopify;
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
        IMigrateLegacyService<Ordre>,
        IExecuteImportFromShopify<Order>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDbContext;
        private readonly ShopifyClientService _shopifyClientService;
        private readonly IProcessImportCommandService<ImportSalesOrderCommand> _processImportService;

        public ImportSalesOrderService(
            CISDbContext dbContext, 
            SWNDistroContext swnDbContext,
            ShopifyClientService shopifyClientService,
            IProcessImportCommandService<ImportSalesOrderCommand> processImportService)
        {
            _dbContext = dbContext;
            _swnDbContext = swnDbContext;
            _shopifyClientService = shopifyClientService;
            _processImportService = processImportService;
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

                var success = await _processImportService.Import(new() 
                { 
                    Definitions = importDefinitions 
                }, CancellationToken.None);

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
    }
}
