using CIS.Application.Features;
using CIS.Application.Hubs;
using CIS.Application.Listeners;
using Microsoft.AspNetCore.SignalR;

namespace CIS.Application.Shared.Infrastructure
{
    public class SignalRNotifyClientService : INotifyClientService
    {
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;

        public SignalRNotifyClientService(IHubContext<ImportLegacyDataHub, IListenImportClient> hub)
        {
            _hub = hub;
        }

        public async Task SendPlainText(string text)
        {
            await _hub.Clients.All.ReceiveMessage(text);
        }
    }
}
