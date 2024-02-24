using CIS.Application;
using CIS.Application.Shared;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Pages.Customer
{
    public partial class Home
    {
        [Inject]
        public required ICISUserService UserService { get; set; }

        private ApplicationUserView? _signedOnUser;

        protected override async Task OnInitializedAsync()
        {
            _signedOnUser = await UserService.GetCurrentUser();
        }
    }
}
