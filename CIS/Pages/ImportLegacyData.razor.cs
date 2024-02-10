using CIS.Application.Legacy;
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
        public SWNDistro LegacyDbContext { get; set; }

        [Inject]
        public ImportLegacyDataBackgroundService BackgroundImportService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        private int _orderCount;
        private int _customerCount;
        private int _productCount;

        private int _legacyOrdersCount;
        private IEnumerable<Ordre> _legacyOrders;

        private int _legacyStoresCount;
        private IEnumerable<Butikkliste> _legacyStores;

        private int _legacyProductCount;
        private IEnumerable<Vareinfo> _legacyProducts;

        private bool _isImporting = false;
        private IEnumerable<string> _logMessages = new List<string>();

        private HubConnection _hubConnection;
        private bool _importDone;

        protected override async Task OnInitializedAsync()
        {
            _orderCount = await LegacyDbContext.Ordres.CountAsync();
            _customerCount = await LegacyDbContext.Butikklistes.CountAsync();
            _productCount = await LegacyDbContext.Vareinfos.CountAsync();

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
        }

        private void ResetDataSources()
        {
            _legacyOrders = null;
            _legacyProducts = null;
            _legacyStores = null;
        }

        private async Task StartBackgroundService()
        {
            _logMessages = new List<string>();
            NotificationService.Notify(
                NotificationSeverity.Info, "Import startet");
            _isImporting = true;
            await InvokeAsync(StateHasChanged);
            await _hubConnection.SendAsync("StartBackgroundService");
        }

        private async Task StopBackgroundService()
        {
            await _hubConnection.SendAsync("StopBackgroundService");
        }

        private async Task LoadLegacyOrders(LoadDataArgs args)
        {
            var query = LegacyDbContext.Ordres
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyOrdersCount = query.Count();
            _legacyOrders = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        private async Task LoadLegacyStores(LoadDataArgs args)
        {
            var query = LegacyDbContext.Butikklistes
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyStoresCount = query.Count();
            _legacyStores = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        private async Task LoadLegacyProducts(LoadDataArgs args)
        {
            var query = LegacyDbContext.Vareinfos
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyProductCount = query.Count();
            _legacyProducts = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        public void Dispose()
        {
            _hubConnection.DisposeAsync();
        }
    }
}
