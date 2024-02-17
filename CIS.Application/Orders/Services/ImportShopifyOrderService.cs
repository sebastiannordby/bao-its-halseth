using CIS.Application.Legacy;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Application.Shopify;
using CIS.Library.Shared.Services;
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
    internal class ImportShopifyOrderService : IExecuteImportFromShopify<Order>
    {
        private readonly CISDbContext _dbContext;
        private readonly ShopifyClientService _shopifyClientService;

        public ImportShopifyOrderService(
            CISDbContext dbContext,
            ShopifyClientService shopifyClientService)
        {
            _dbContext = dbContext;
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
    }
}
