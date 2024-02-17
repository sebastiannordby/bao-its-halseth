using CIS.Application.Shopify.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace CIS.Application.Shopify
{
    public class ShopifyClientService
    {
        private readonly ShopifyClientServiceOptions _options;
        private readonly ShopifyApiCredentials _apiCredentials;


        private readonly IOrderService _orderService;
        private readonly IDraftOrderService _draftOrderService;

        public ShopifyClientService(IOptions<ShopifyClientServiceOptions> options, IOrderServiceFactory orderServiceFactory, IDraftOrderServiceFactory draftOrderServiceFactory )
        {
            _options = options.Value;

            //trenger accesstoken :)))))
            _apiCredentials = new ShopifyApiCredentials($"https://{_options.Domain}.myshopify.com", _options.AccessToken);
            _orderService = orderServiceFactory.Create(_apiCredentials);
            _draftOrderService = draftOrderServiceFactory.Create(_apiCredentials);

        }


        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var orders = await _orderService.ListAsync();
            return orders.Items;
        }


        public async Task<IEnumerable<DraftOrder>> GetDraftOrdersAsync()
        {
            var draftOrders = await _draftOrderService.ListAsync();
            return draftOrders.Items;
        }


    }
}
