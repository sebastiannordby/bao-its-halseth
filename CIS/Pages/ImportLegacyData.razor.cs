using CIS.Application.Legacy;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CIS.Pages
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

        protected override async Task OnInitializedAsync()
        {
            if(!BackgroundImportService.IsRunning)
            {
                _migrationTasks = await MigrationTaskRepo.GetMigrationTasks();
                _uncompletedMigrationTasks = _migrationTasks
                    .Where(x => !x.Executed)
                    .Select(x => x.Type)
                    .ToList();

                _orderCount = await LegacyDbContext.Ordres.CountAsync();
                _customerCount = await LegacyDbContext.Butikklistes.CountAsync();
                _productCount = await LegacyDbContext.Vareinfos.CountAsync();
                _salesStatisticsCount = await LegacyDbContext.Salgs.CountAsync();
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

            _hubConnection.On<string>("ReceiveMessage", async msg =>
            {
                _logMessages = _logMessages.Prepend(msg);
                await InvokeAsync(StateHasChanged);
            });

            _hubConnection.On("Finished", () =>
            {
                NotificationService.Notify(NotificationSeverity.Info, "Import ferdig",
                    "Importering av data ferdig.");
                _importDone = true;
                _logMessages = _logMessages.Prepend("Importering av data ferdig!");
                _logMessages = _logMessages.Prepend("Naviger deg via side menyen for å se de nylige importerte dataene.");
                _logMessages = _logMessages.Prepend("🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳🥳");
            });

            await _hubConnection.StartAsync();
            _logMessages = new List<string>();
            _isImporting = true;
        }

        private async Task StartBackgroundService()
        {
            await SetToImportingState();
            NotificationService.Notify(
                NotificationSeverity.Info, "Import startet");
            await InvokeAsync(StateHasChanged);
            await _hubConnection.SendAsync("StartBackgroundService");
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
        }
    }
}
