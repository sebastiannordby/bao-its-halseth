using CIS.Application.Shared.Services;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using ShopifyOrder = ShopifySharp.Order;

namespace CIS.Application.Shopify
{
    public class ImportShopifyOrderTimedWorker(
        IExecuteImportFromShopify<ShopifyOrder> orderService,
        ILogger<ImportShopifyOrderTimedWorker> logger) : IInvocable
    {
        public async Task Invoke()
        {
            logger.LogDebug("({WorkerName}) Worker started at: {Time}.",
                nameof(ImportShopifyOrderTimedWorker), DateTimeOffset.Now);

            await orderService.ExecuteShopifyImport();

            logger.LogDebug("({WorkerName}) Finished execution at: {Time}.",
                nameof(ImportShopifyOrderTimedWorker), DateTimeOffset.Now);
        }
    }
}
