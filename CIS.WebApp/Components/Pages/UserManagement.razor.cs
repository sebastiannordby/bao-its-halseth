using CIS.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class UserManagement : ComponentBase
    {
        [Inject]
        public required UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public required RoleManager<IdentityRole> RoleManager { get; set; }

        private NewUserModel NewUser = new NewUserModel();

        private IEnumerable<ApplicationUser> _users = Enumerable.Empty<ApplicationUser>();

        protected override async Task OnInitializedAsync()
        {
            var users = await UserManager.Users
                .ToListAsync();

            _users = users;
        }

        private async Task HandleValidSubmit()
        {
            var user = new ApplicationUser { UserName = NewUser.UserName };
            var result = await UserManager.CreateAsync(user, NewUser.Password);

            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user, NewUser.Role);
            }

            NewUser = new();
        }

        private class NewUserModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string Role { get; set; }
        }
    }
}
