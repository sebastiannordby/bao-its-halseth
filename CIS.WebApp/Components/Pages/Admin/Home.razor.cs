using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Infrastructure;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Home
    {
        [Inject]
        public IMigrationTaskRepo MigrationTaskRepo { get; set; }

        [Inject]
        public ISalesQueries SalesQueries { get; set; }

        private IEnumerable<MigrationTask> _migrationTasks;
        private IEnumerable<MigrationTask> _uncompletedMigrationTasks;
        private IReadOnlyCollection<MostSoldProductView> _mostSoldProducts;
        private IReadOnlyCollection<StoreMostBoughtView> _bestCustomerStores;
        private DateTime _currentSeasonDate;
        private SeasonalityAnalysisResult _seasonalAnalysis;
        private readonly Dictionary<MigrationTask.TaskType, string> _migrationTaskTypeNames = new()
        {
            { MigrationTask.TaskType.Customers, "Kunde" },
            { MigrationTask.TaskType.Products, "Varer" },
            { MigrationTask.TaskType.SalesOrders, "Bestillinger" }
        };

        private bool _showMigrationPage = true;

        protected override async Task OnInitializedAsync()
        {
            _migrationTasks = await MigrationTaskRepo.GetMigrationTasks();
            _uncompletedMigrationTasks = _migrationTasks.Where(x => !x.Executed);

            _showMigrationPage = _uncompletedMigrationTasks.Any();

            if (!_showMigrationPage)
            {
                _mostSoldProducts = await SalesQueries
                    .GetMostSoldProduct(5);
                _bestCustomerStores = await SalesQueries
                    .GetMostBoughtViews(5);
            }
        }
    }
}
