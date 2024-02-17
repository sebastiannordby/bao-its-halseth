using CIS.Application.Listeners;
using Microsoft.AspNetCore.SignalR;

namespace CIS.Application.Hubs
{
    public class ImportLegacyDataHub : Hub<IListenImportClient>
    {

    }
}
