using CIS.Application.Features.Stores.Models;
using CIS.Application.Features.Stores.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Stores : ComponentBase, IDisposable
    {
        [Inject]
        public required IStoreService StoreViewRepository { get; set; }

        private IReadOnlyCollection<StoreView> _stores = ReadOnlyCollection<StoreView>.Empty;
        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _stores = await StoreViewRepository
                .List(_cts.Token);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
