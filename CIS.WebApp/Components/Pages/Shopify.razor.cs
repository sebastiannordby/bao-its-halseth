using CIS.Application.Shopify;
using Microsoft.AspNetCore.Components;
using Radzen;
using ShopifySharp;

namespace CIS.WebApp.Components.Pages
{
    public partial class Shopify : ComponentBase
    {
        [Inject]
        public required ShopifyClientService ShopifyService { get; set; }

        private IEnumerable<DraftOrder> _draftOrders = Enumerable.Empty<DraftOrder>();
        private int _draftOrdersCount;

        private IEnumerable<Order> _orders = Enumerable.Empty<Order>();
        private int _ordersCount;

        private async Task LoadOrders(LoadDataArgs args)
        {
            var orders = await ShopifyService
                .GetOrdersAsync(DateTime.Now.AddYears(-1));

            _orders = orders;
            _ordersCount = orders.Count();
        }

        private async Task LoadDraftOrders(LoadDataArgs args)
        {
            var draftOrders = await ShopifyService
                .GetDraftOrdersAsync();

            _draftOrders = draftOrders;
            _draftOrdersCount = draftOrders.Count();
        }
    }
}
