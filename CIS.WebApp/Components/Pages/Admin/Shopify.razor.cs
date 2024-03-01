using CIS.Application.Shopify;
using Microsoft.AspNetCore.Components;
using Radzen;
using ShopifySharp;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Shopify : ComponentBase, IDisposable
    {
        [Inject]
        public required IShopifyClientService ShopifyService { get; set; }

        private IEnumerable<DraftOrder> _draftOrders = Enumerable.Empty<DraftOrder>();
        private int _draftOrdersCount;

        private IEnumerable<Order> _orders = Enumerable.Empty<Order>();
        private int _ordersCount;

        private CancellationTokenSource _cts = new();

        private async Task LoadOrders(LoadDataArgs args)
        {
            var orders = await ShopifyService
                .GetOrdersAsync(_cts.Token);

            _orders = orders;
            _ordersCount = orders.Count();
        }

        private async Task LoadDraftOrders(LoadDataArgs args)
        {
            var draftOrders = await ShopifyService
                .GetDraftOrdersAsync(_cts.Token);

            _draftOrders = draftOrders;
            _draftOrdersCount = draftOrders.Count();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
