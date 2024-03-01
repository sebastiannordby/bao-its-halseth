
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Layout
{
    public partial class NavMenu : IDisposable
    {
        [Inject]
        public required IMigrationTaskRepo MigrationTaskRepo { get; set; }

        private bool _showMigrationPage;

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            var migrationTasks = await MigrationTaskRepo
                .GetMigrationTasks(_cts.Token);

            _showMigrationPage = migrationTasks
                .Where(x => !x.Executed)
                .Any();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
