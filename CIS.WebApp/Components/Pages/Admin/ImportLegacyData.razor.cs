using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.WebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.SignalR.Client;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class ImportLegacyData : ComponentBase, IDisposable
    {
        [Inject]
        public SWNDistroContext LegacyDbContext { get; set; }

        [Inject]
        public ImportLegacyDataBackgroundService BackgroundImportService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        [Inject]
        public IMigrationTaskRepo MigrationTaskRepo { get; set; }

        private int _orderCount;
        private int _customerCount;
        private int _productCount;
        private int _salesStatisticsCount;

        private bool _isImporting = false;
        private IEnumerable<string> _logMessages = new List<string>();

        private IEnumerable<MigrationTask> _migrationTasks;
        private IEnumerable<MigrationTask.TaskType> _uncompletedMigrationTasks;

        private bool ShowCustomers => _uncompletedMigrationTasks
            .Any(x => x == MigrationTask.TaskType.Customers);
        private bool ShowProducts => _uncompletedMigrationTasks
            .Any(x => x == MigrationTask.TaskType.Products);
        private bool ShowSalesOrders => _uncompletedMigrationTasks
            .Any(x => x == MigrationTask.TaskType.SalesOrders);
        private bool ShowSalesStatistics => _uncompletedMigrationTasks
            .Any(x => x == MigrationTask.TaskType.SalesOrderStatistics);

        private HubConnection _hubConnection;
        private bool _importDone;

        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            if(!BackgroundImportService.IsRunning)
            {
                _migrationTasks = await MigrationTaskRepo.GetMigrationTasks(_cts.Token);
                _uncompletedMigrationTasks = _migrationTasks
                    .Where(x => !x.Executed)
                    .Select(x => x.Type)
                    .ToList();

                _orderCount = await LegacyDbContext.Ordres.CountAsync(_cts.Token);
                _customerCount = await LegacyDbContext.Butikklistes.CountAsync(_cts.Token);
                _productCount = await LegacyDbContext.Vareinfos.CountAsync(_cts.Token);
                _salesStatisticsCount = await LegacyDbContext.Salgs.CountAsync(_cts.Token);
            }
            else
            {
                await SetToImportingState();
            }
        }

        private async Task SetToImportingState()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/import-legacy-hub")) // Replace "/myHub" with the appropriate endpoint URL
                .Build();

            _hubConnection.On<string>(nameof(IListenImportClient.ReceiveMessage), async msg =>
            {
                _logMessages = _logMessages.Prepend(msg);
                await InvokeAsync(StateHasChanged);
            });

            _hubConnection.On(nameof(IListenImportClient.Finished), async() =>
            {
                NotificationService.Notify(NotificationSeverity.Info, "Import ferdig",
                    "Importering av data ferdig.");
                _importDone = true;
                _logMessages = _logMessages.Prepend("Importering av data ferdig!");
                _logMessages = _logMessages.Prepend("Naviger deg via side menyen for å se de nylige importerte dataene.");
                _logMessages = _logMessages.Prepend("🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳");
                await InvokeAsync(StateHasChanged);
            });

            await _hubConnection.StartAsync(_cts.Token);
            _logMessages = new List<string>();
            _isImporting = true;
        }

        private async Task StartBackgroundService()
        {
            await SetToImportingState();
            NotificationService.Notify(
                NotificationSeverity.Info, "Import startet");
            await BackgroundImportService.StartAsync(CancellationToken.None);
        }

        private async Task StopBackgroundService()
        {
            await _hubConnection.SendAsync("StopBackgroundService");
        }

        public void Dispose()
        {
            if(_hubConnection is not null)
            {
                _hubConnection.DisposeAsync();
            }

            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
