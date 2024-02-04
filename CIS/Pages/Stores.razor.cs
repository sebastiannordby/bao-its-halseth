using CIS.Library.Stores.Models;
using CIS.Library.Stores.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.Pages
{
    public partial class Stores : ComponentBase
    {
        [Inject]
        private IStoreViewRepository StoreViewRepository { get; set; }

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
