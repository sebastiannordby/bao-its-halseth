using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Home : IDisposable
    {
        [Inject]
        public required IMigrationTaskRepo MigrationTaskRepo { get; set; }

        [Inject]
        public required ISalesQueries SalesQueries { get; set; }

        private IReadOnlyCollection<MostSoldProductView> _mostSoldProducts = 
            ReadOnlyCollection<MostSoldProductView>.Empty;
        
        private IReadOnlyCollection<StoreMostBoughtView> _bestCustomerStores =
            ReadOnlyCollection<StoreMostBoughtView>.Empty;
        
        private DateTime _currentSeasonDate;
        private readonly Dictionary<MigrationTask.TaskType, string> _migrationTaskTypeNames = new()
        {
            { MigrationTask.TaskType.Customers, "Kunde" },
            { MigrationTask.TaskType.Products, "Varer" },
            { MigrationTask.TaskType.SalesOrders, "Bestillinger" }
        };

        private bool _showMigrationPage = true;

        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            _showMigrationPage = await MigrationTaskRepo
                .IsAllMigrationsExecuted(_cts.Token) == false;

            if (!_showMigrationPage)
            {
                _mostSoldProducts = await SalesQueries
                    .GetMostSoldProduct(5);
                _bestCustomerStores = await SalesQueries
                    .GetMostBoughtViews(5);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
