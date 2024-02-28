using CIS.Application.Legacy;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using ShopifySharp;
using ShopifySharp.GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using ShopifyOrder = ShopifySharp.Order;
using CIS.Application.Features.Orders.Infrastructure.Models;

namespace CIS.Application.Shopify
{
    internal class ImportShopifyOrderService : IExecuteImportFromShopify<ShopifyOrder>
    {
        private readonly CISDbContext _dbContext;
        private readonly IShopifyClientService _shopifyClientService;

        public ImportShopifyOrderService(
            CISDbContext dbContext,
            IShopifyClientService shopifyClientService)
        {
            _dbContext = dbContext;
            _shopifyClientService = shopifyClientService;
        }

        public async Task ExecuteShopifyImport(CancellationToken cancellationToken = default)
        {
            var latestOrderDate = _dbContext.SalesOrders
                .Max(x => x.OrderDate);

            var shopifyOrders = await _shopifyClientService.GetOrdersAsync(
                cancellationToken);

            var ordersToInsert = new List<SalesOrderDao>();
            var orderLinesToInsert = new List<SalesOrderLineDao>();

            foreach (var shopifyOrder in shopifyOrders)
            {
                var newOrder = new SalesOrderDao()
                {
                    Id = Guid.NewGuid(),
                    Number = shopifyOrder.OrderNumber ?? 0,
                    AlternateNumber = shopifyOrder.OrderNumber?.ToString(),
                    CustomerName = shopifyOrder.Customer.FirstName,
                    StoreName = shopifyOrder.Customer.LastName,
                    OrderDate = shopifyOrder.CreatedAt.HasValue ?
                        DateOnly.FromDateTime(shopifyOrder.CreatedAt.Value.DateTime) : DateOnly.FromDateTime(DateTime.Now),
                };

                var newOrderLines = new List<SalesOrderLineDao>();
                var shopifyProductsIdsUsed = shopifyOrder.LineItems
                    .Select(x => x.ProductId);
                var productQuery = _dbContext.Products
                    .AsNoTracking()
                    .Where(x => shopifyProductsIdsUsed.Contains(x.AlternateNumber));

                var productDetails = await (
                    from product in productQuery
                    join price in _dbContext.ProductPrices
                        on product.ProductPriceId equals price.Id
                    select new
                    {
                        Product = product,
                        Price = price
                    }
                ).ToListAsync();

                foreach (var shopifyLine in shopifyOrder.LineItems)
                {
                    var productDetail = productDetails
                        .First(x => x.Product.AlternateNumber == shopifyLine.Id);

                    var newOrderLine = new SalesOrderLineDao()
                    {
                        ProductNumber = productDetail.Product.Number,
                        ProductName = productDetail.Product.Name,
                        EAN = productDetail.Product.EAN,
                        CostPrice = productDetail.Price.CostPrice,
                        PurchasePrice = productDetail.Price.PurchasePrice,
                        StorePrice = productDetail.Price.StorePrice,
                        Quantity = shopifyLine.Quantity ?? 1,
                        QuantityDelivered = 0,
                        CurrencyCode = "NOK",
                        SalesOrderId = newOrder.Id
                    };
                }

                await _dbContext.SalesOrders.AddAsync(newOrder);
                await _dbContext.AddRangeAsync(newOrderLines);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
