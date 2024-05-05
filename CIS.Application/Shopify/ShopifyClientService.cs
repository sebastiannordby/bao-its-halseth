using CIS.Application.Shopify.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using ShopifySharp.Filters;
using System.Threading;

namespace CIS.Application.Shopify
{
    public interface IShopifyClientService
    {
        Task<IEnumerable<Order>> GetOrdersAsync(CancellationToken cancellationToken);
        Task<IEnumerable<DraftOrder>> GetDraftOrdersAsync(CancellationToken cancellationToken);

        public static void RegisterShopifyClientService(IServiceCollection register)
        {
            register.AddScoped<IShopifyClientService, ShopifyClientService>();
        }

        private class ShopifyClientService : IShopifyClientService
        {
            private readonly ShopifyClientServiceOptions _options;
            private readonly ShopifyApiCredentials _apiCredentials;
            private readonly IOrderService _orderService;
            private readonly IDraftOrderService _draftOrderService;

            public ShopifyClientService(
                IOptions<ShopifyClientServiceOptions> options,
                IOrderServiceFactory orderServiceFactory,
                IDraftOrderServiceFactory draftOrderServiceFactory)
            {
                _options = options.Value;
                _apiCredentials = new ShopifyApiCredentials(
                    shopDomain: $"https://{_options.Domain}.myshopify.com",
                    accessToken: _options.AccessToken);
                _orderService = orderServiceFactory.Create(_apiCredentials);
                _draftOrderService = draftOrderServiceFactory.Create(_apiCredentials);
            }

            public async Task<IEnumerable<Order>> GetOrdersAsync(
                CancellationToken cancellationToken)
            {
                var filter = new OrderListFilter()
                {
                    Limit = 100
                };

                var orders = await _orderService
                    .ListAsync(filter, cancellationToken);

                return orders.Items;
            }

            public async Task<IEnumerable<DraftOrder>> GetDraftOrdersAsync(
                CancellationToken cancellationToken)
            {
                var draftOrders = await _draftOrderService
                    .ListAsync(null, cancellationToken);

                return draftOrders.Items;
            }
        }
    }
}
