using CIS.Application;
using CIS.Application.Shared;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Pages.Customer
{
    public partial class Home : IDisposable
    {
        [Inject]
        public required ICISUserService UserService { get; set; }

        private ApplicationUserView? _signedOnUser;

        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            _signedOnUser = await UserService
                .GetCurrentUser(_cts.Token);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
