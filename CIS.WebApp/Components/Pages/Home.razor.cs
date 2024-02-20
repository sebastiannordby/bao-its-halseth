using CIS.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace CIS.WebApp.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        public required IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public required UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var user = await UserManager.GetUserAsync(
                HttpContextAccessor.HttpContext.User);

            if(user is not null)
            {
                var roles = await UserManager.GetRolesAsync(user);

                if(roles.Contains(UserTypes.ADMINISTRATOR))
                {
                    NavigationManager.NavigateTo("/admin/home");
                }
                else if (roles.Contains(UserTypes.CUSTOMER))
                {
                    NavigationManager.NavigateTo("/customer/home");
                }
            }

        }
    }
}
