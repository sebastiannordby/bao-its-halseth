﻿using CIS.Application;
using CIS.Application.Products.Models;
using CIS.Application.Stores.Models;
using CIS.Application.Stores.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CIS.WebApp.Components.Pages.Customer
{
    public partial class StockCount
    {
        [Inject]
        public required IStockCountService StockCountService { get; set; }

        [Inject]
        public required CISUserService UserService { get; set; }

        [Inject]
        public CISDbContext DbContext { get; set; }

        private Guid _storeId;
        private ApplicationUser _currentUser;
        private IEnumerable<StockCountView> _currentStoreCountDataSource = Enumerable.Empty<StockCountView>();
        private IEnumerable<StockCountView> _historyStoreCountDataSource = Enumerable.Empty<StockCountView>();
        private RegisterStockCountInput _registerInput = new();

        private IEnumerable<ProductDao> _products = Enumerable.Empty<ProductDao>(); // Not to this

        protected override async Task OnInitializedAsync()
        {
            _storeId = await UserService.GetCurrentStoreId();
            _currentUser = await UserService.GetCurrentUser();

            _products = await DbContext.Products.ToListAsync();

            await FetchCurrentStoreCount();
        }

        private async Task FetchCurrentStoreCount()
        {
            _currentStoreCountDataSource = await StockCountService
                .GetByStore(_storeId);

            _historyStoreCountDataSource = await StockCountService
                .GetHistoryByStore(_storeId);
        }

        private async Task ExecuteRegisterStockCount()
        {
            await StockCountService.AddStockCount(
                _registerInput.ProductId,
                _storeId,
                _registerInput.Quantity,
                _currentUser.UserName ?? "UGYLDIG");

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