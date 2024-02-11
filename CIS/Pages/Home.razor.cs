using CIS.Application.Orders.Models;
using CIS.Application.Orders.Repositories;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.Pages
{
    public partial class Home : ComponentBase
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

            _showMigrationPage = _uncompletedMigrationTasks.Count() > 1;

            if(!_showMigrationPage) 
            { 
                _mostSoldProducts = await SalesQueries
                    .GetMostSoldProduct(5);
                _bestCustomerStores = await SalesQueries
                    .GetMostBoughtViews(5);

                _currentSeasonDate = DateTime.Now.AddYears(-1);
                //_seasonalAnalysis = await SalesQueries
                //    .AnalyzeSeasonality(_currentSeasonDate.Year);
            }
        }
    }
}
