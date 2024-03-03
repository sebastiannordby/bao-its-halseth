
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Layout
{
    public partial class NavMenu : IDisposable
    {
        [Inject]
        public required IMigrationTaskRepo MigrationTaskRepo { get; set; }

        private readonly CancellationTokenSource _cts = new();
        private bool _showMigrationPage;

        protected override async Task OnInitializedAsync()
        {
            _showMigrationPage = await MigrationTaskRepo
                .IsAllMigrationsExecuted(_cts.Token) == false;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
