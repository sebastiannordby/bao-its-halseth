using CIS.Application.Shared.Services;
using Microsoft.EntityFrameworkCore;
using ShopifyOrder = ShopifySharp.Order;
using CIS.Application.Features.Orders.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace CIS.Application.Shopify
{
    internal class ImportShopifyOrderService : IExecuteImportFromShopify<ShopifyOrder>
    {
        private readonly CISDbContext _dbContext;
        private readonly IShopifyClientService _shopifyClientService;
        private readonly ILogger<ImportShopifyOrderService> _logger;

        public ImportShopifyOrderService(
            CISDbContext dbContext,
            IShopifyClientService shopifyClientService,
            ILogger<ImportShopifyOrderService> logger)
        {
            _dbContext = dbContext;
            _shopifyClientService = shopifyClientService;
            _logger = logger;
        }

        public async Task ExecuteShopifyImport(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing import of orders from shopify.");

            try
            {
                var shopifyOrders = await _shopifyClientService
                    .GetOrdersAsync(cancellationToken);

                var ordersToInsert = new List<SalesOrderDao>();
                var orderLinesToInsert = new List<SalesOrderLineDao>();

                foreach (var shopifyOrder in shopifyOrders)
                {
                    var orderExists = await _dbContext.SalesOrders
                        .AnyAsync(x => x.Number == shopifyOrder.Number, cancellationToken);
                    if (orderExists)
                        continue;

                    var newOrder = new SalesOrderDao()
                    {
                        Id = Guid.NewGuid(),
                        Number = shopifyOrder.Number ?? 0,
                        AlternateNumber = shopifyOrder.OrderNumber?.ToString(),
                        CustomerName = shopifyOrder.Customer.FirstName,
                        StoreName = shopifyOrder.Customer.LastName,
                        OrderDate = shopifyOrder.CreatedAt.HasValue ?
                            shopifyOrder.CreatedAt.Value.DateTime : DateTime.Now,
                    };

                    ordersToInsert.Add(newOrder);

                    var newOrderLines = new List<SalesOrderLineDao>();
                    var shopifySkuNumbers = shopifyOrder.LineItems
                        .Select(x => x.SKU.ToString());
                    var productQuery = _dbContext.Products
                        .AsNoTracking()
                        .Where(x => shopifySkuNumbers.Contains(x.SuppliersProductNumber));

                    var productDetails = await (
                        from product in productQuery
                        join price in _dbContext.ProductPrices
                            on product.ProductPriceId equals price.Id
                        select new
                        {
                            Product = product,
                            Price = price
                        }
                    ).ToListAsync(cancellationToken);

                    foreach (var shopifyLine in shopifyOrder.LineItems)
                    {
                        var productDetail = productDetails
                            .First(x => x.Product.SuppliersProductNumber == shopifyLine.SKU.ToString());

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

                        orderLinesToInsert.Add(newOrderLine);
                    }
                }

                if (ordersToInsert.Count > 0)
                {
                    _logger.LogInformation(
                        "Inserting Orders={OrderCount} and OrderLines={OrderLineCount}",
                        ordersToInsert.Count,
                        orderLinesToInsert.Count);

                    await _dbContext.SalesOrders.AddRangeAsync(ordersToInsert, cancellationToken);
                    await _dbContext.AddRangeAsync(orderLinesToInsert, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, nameof(ImportShopifyOrderService));
            }

            _logger.LogInformation("Executing import of orders from shopify exited successfully.");
        }
    }
}
