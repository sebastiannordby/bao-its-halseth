using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        public IMigrationTaskRepo MigrationTaskRepo { get; set; }

        private IEnumerable<MigrationTask> _migrationTasks;
        private IEnumerable<MigrationTask> _uncompletedMigrationTasks;

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
        }
    }
}
