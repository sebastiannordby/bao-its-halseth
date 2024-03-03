using CIS.Application.Shared.Services;
using Microsoft.EntityFrameworkCore;
using ShopifyOrder = ShopifySharp.Order;

namespace CIS.Integration
{
    public class ShopifyWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ShopifyWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderService = scope.ServiceProvider
                        .GetRequiredService<IExecuteImportFromShopify<ShopifyOrder>>();
                    var logger = scope.ServiceProvider
                        .GetRequiredService<ILogger<ShopifyWorker>>();

                    logger.LogDebug("(ShopifyWorker) Worker running at: {time}", 
                        DateTimeOffset.Now);

                    await orderService.ExecuteShopifyImport(stoppingToken);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
