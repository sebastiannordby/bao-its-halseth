using CIS.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CIS.WebApp.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public required IHttpContextAccessor HttpContextAccessor { get; set; }

        public async Task SignOut()
        {
            var authState = await AuthenticationStateProvider
                .GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated == true && HttpContextAccessor.HttpContext is not null)
            {
                await HttpContextAccessor.HttpContext.SignOutAsync();
            }
        }
    }
}
