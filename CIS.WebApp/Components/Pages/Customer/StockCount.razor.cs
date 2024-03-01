using CIS.Application;
using CIS.Application.Features.Products;
using CIS.Application.Features.Products.Models;
using CIS.Application.Features.Stores.Models;
using CIS.Application.Features.Stores.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CIS.WebApp.Components.Pages.Customer
{
    public partial class StockCount
    {
        [Inject]
        public required IStockCountService StockCountService { get; set; }

        [Inject]
        public required ICISUserService UserService { get; set; }

        [Inject]
        public required IProductQueries ProductQueries { get; set; }

        private Guid _storeId;
        private ApplicationUser _currentUser;
        private IEnumerable<StockCountView> _currentStoreCountDataSource = Enumerable.Empty<StockCountView>();
        private IEnumerable<StockCountView> _historyStoreCountDataSource = Enumerable.Empty<StockCountView>();
        private RegisterStockCountInput _registerInput = new();

        private IEnumerable<ProductView> _products = Enumerable.Empty<ProductView>(); // Not to this

        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            _storeId = await UserService.GetCurrentStoreId(_cts.Token);
            _currentUser = await UserService.GetCurrentUser(_cts.Token);

            _products = await ProductQueries.List(_cts.Token);

            await FetchCurrentStoreCount();
        }

        private async Task FetchCurrentStoreCount()
        {
            _currentStoreCountDataSource = await StockCountService
                .GetByStore(_storeId, _cts.Token);

            _historyStoreCountDataSource = await StockCountService
                .GetHistoryByStore(_storeId, _cts.Token);
        }

        private async Task ExecuteRegisterStockCount()
        {
            await StockCountService.AddStockCount(
                _registerInput.ProductId,
                _storeId,
                _registerInput.Quantity,
                _currentUser.UserName ?? "UGYLDIG",
                _cts.Token);

            _registerInput = new();
            await FetchCurrentStoreCount();
        }

        private class RegisterStockCountInput
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
