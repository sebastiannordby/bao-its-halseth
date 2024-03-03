using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Home 
    {
        [Inject]
        public required IMigrationTaskRepo MigrationTaskRepo { get; set; }

        [Inject]
        public required ISalesQueries SalesQueries { get; set; }

        [CascadingParameter(Name = "ShowMigrationPage")]
        public bool ShowMigrationPage { get;set; }

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


        protected override async Task OnInitializedAsync()
        {
            if (!ShowMigrationPage)
            {
                _mostSoldProducts = await SalesQueries
                    .GetMostSoldProduct(5);
                _bestCustomerStores = await SalesQueries
                    .GetMostBoughtViews(5);
            }
        }
    }
}
