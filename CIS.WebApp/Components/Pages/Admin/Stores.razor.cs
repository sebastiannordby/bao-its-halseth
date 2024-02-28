using CIS.Application.Features.Stores.Models;
using CIS.Application.Features.Stores.Services;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Stores : ComponentBase
    {
        [Inject]
        private IStoreService StoreViewRepository { get; set; }

        private IReadOnlyCollection<StoreView> _stores;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _stores = await StoreViewRepository.List();
        }
    }
}
