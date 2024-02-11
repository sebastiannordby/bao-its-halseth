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
        public ISalesOrderViewRepository SalesOrderViewRepository { get; set; }

        private IEnumerable<MigrationTask> _migrationTasks;
        private IEnumerable<MigrationTask> _uncompletedMigrationTasks;
        private IReadOnlyCollection<MostSoldProductView> _mostSoldProducts;
        private IReadOnlyCollection<StoreMostBoughtView> _bestCustomerStores;
        private readonly Dictionary<MigrationTask.TaskType, string> _migrationTaskTypeNames = new()
        {
            { MigrationTask.TaskType.Customers, "Kunde" },
            { MigrationTask.TaskType.Products, "Varer" },
            { MigrationTask.TaskType.SalesOrders, "Bestillinger" }
        };

        protected override async Task OnInitializedAsync()
        {
            _migrationTasks = await MigrationTaskRepo.GetMigrationTasks();
            _uncompletedMigrationTasks = _migrationTasks.Where(x => !x.Executed);

            _mostSoldProducts = await SalesOrderViewRepository
                .GetMostSoldProduct(5);
            _bestCustomerStores = await SalesOrderViewRepository
                .GetMostBoughtViews(5);
        }
    }
}
