
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Layout
{
    public partial class NavMenu
    {
        [Inject]
        public required IMigrationTaskRepo MigrationTaskRepo { get; set; }

        private bool _showMigrationPage;
        
        protected override async Task OnInitializedAsync()
        {
            var migrationTasks = await MigrationTaskRepo
                .GetMigrationTasks();

            _showMigrationPage = migrationTasks
                .Where(x => !x.Executed)
                .Any();
        }
    }
}
