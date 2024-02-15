using CIS.Application.Shopify.Options;
using Microsoft.Extensions.Options;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;

namespace CIS.Application.Shopify
{
    public class ShopifyClientService
    {
        private readonly ShopifyClientServiceOptions _options;
        private readonly IOrderServiceFactory _orderServiceFactory;
        private readonly ShopifyApiCredentials _apiCredentials;
        private readonly IOrderService _orderService;


        public ShopifyClientService(IOptions<ShopifyClientServiceOptions> options, IOrderServiceFactory orderServiceFactory)
        {
            _options = options.Value;
            _orderServiceFactory = orderServiceFactory;

            //trenger accesstoken :)))))
            _apiCredentials = new ShopifyApiCredentials(_options.Domain, _options.AccessToken);
            _orderService = _orderServiceFactory.Create(_apiCredentials);

        }


        public async Task<IEnumerable<Order>> GetOrders()
        {
            var orders = await _orderService.ListAsync();

            return orders.Items;
        }

    }
}
