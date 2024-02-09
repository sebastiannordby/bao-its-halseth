using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace CIS.Services
{
    public class ImportLegacyDataHub : Hub
    {
        private readonly ImportLegacyDataBackgroundService _backgroundService;

        public ImportLegacyDataHub(ImportLegacyDataBackgroundService backgroundService)
        {
            _backgroundService = backgroundService;
        }

        // This method can be used if you need to handle client connections or disconnections
        public override async Task OnConnectedAsync()
        {
            // Perform any necessary actions when a client connects to the hub
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Perform any necessary actions when a client disconnects from the hub
            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartBackgroundService()
        {
            await _backgroundService.StartAsync(CancellationToken.None);
        }

        public async Task StopBackgroundService()
        {
            await _backgroundService.StopAsync(CancellationToken.None);
        }
    }
}
