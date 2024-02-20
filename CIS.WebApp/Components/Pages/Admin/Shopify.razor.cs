using CIS.Application.Shopify;
using Microsoft.AspNetCore.Components;
using Radzen;
using ShopifySharp;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Shopify : ComponentBase
    {
        [Inject]
        public required IShopifyClientService ShopifyService { get; set; }

        private IEnumerable<DraftOrder> _draftOrders = Enumerable.Empty<DraftOrder>();
        private int _draftOrdersCount;

        private IEnumerable<Order> _orders = Enumerable.Empty<Order>();
        private int _ordersCount;

        private async Task LoadOrders(LoadDataArgs args)
        {
            var orders = await ShopifyService
                .GetOrdersAsync(CancellationToken.None);

            _orders = orders;
            _ordersCount = orders.Count();
        }

        private async Task LoadDraftOrders(LoadDataArgs args)
        {
            var draftOrders = await ShopifyService
                .GetDraftOrdersAsync(CancellationToken.None);

            _draftOrders = draftOrders;
            _draftOrdersCount = draftOrders.Count();
        }
    }
}
