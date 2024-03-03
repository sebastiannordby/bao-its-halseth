
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Layout
{
    public partial class NavMenu : IDisposable
    {
        [Inject]
        public required IMigrationTaskRepo MigrationTaskRepo { get; set; }

        [CascadingParameter] public bool ShowMigrationPage { get; set; }

        private readonly CancellationTokenSource _cts = new();

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
