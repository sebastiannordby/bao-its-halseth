using CIS.DataAccess.Legacy;
using ExcelDataReader.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CIS.Services 
{
    public class ImportLegacyDataBackgroundService : BackgroundService
    {
        private readonly ManualResetEvent _signal = new ManualResetEvent(false);
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IHubContext<ImportLegacyDataHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _isRunning;

        public ImportLegacyDataBackgroundService(
            IServiceScopeFactory scopeFactory,
            IHubContext<ImportLegacyDataHub> hubContext)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _signal.WaitOne();
            
            while (!stoppingToken.IsCancellationRequested)
            {

                // Background service logic goes here
                await Task.Delay(1000, stoppingToken); // Simulated work
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            using var scope = _scopeFactory.CreateScope();

            var legacyDbContext = scope.ServiceProvider
                .GetRequiredService<SWNDistro>();

            await Task.Delay(2000);
            await ImportCustomers(legacyDbContext);
            await ImportProducts(legacyDbContext);
        }

        private async Task ImportProducts(SWNDistro legacyDbContext)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
            await Task.Delay(100);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
            await Task.Delay(100);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
            await Task.Delay(100);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
        }

        private async Task ImportCustomers(SWNDistro legacyDbContext)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer kunder/butikker..");
            await Task.Delay(100);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
            await Task.Delay(100);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
            await Task.Delay(100);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Importerer varer..");
        }
    }
}