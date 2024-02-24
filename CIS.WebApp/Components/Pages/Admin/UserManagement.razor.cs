using CIS.Application;
using CIS.Application.Stores.Infrastructure;
using CIS.Application.Stores.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.ComponentModel.DataAnnotations;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class UserManagement : ComponentBase
    {
        [Inject]
        public required UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public required RoleManager<IdentityRole> RoleManager { get; set; }

        [Inject]
        public required NotificationService NotificationService { get; set; }

        [Inject]
        public required IStoreQueries CustomerQueries { get; set; }

        private NewUserModel NewUser = new NewUserModel();

        private IEnumerable<UserView> _userViews = Enumerable.Empty<UserView>();
        private IEnumerable<string> _roles = Enumerable.Empty<string>();
        private IEnumerable<CustomerView> _customers = Enumerable.Empty<CustomerView>();

        protected override async Task OnInitializedAsync()
        {
            await FetchCustomers();
            await FetchUsers();
            await FetchRoles();
        }

        private async Task FetchUsers()
        {
            var users = await UserManager.Users
                .ToListAsync();

            var userViews = users.Select(user =>
            {
                var customer = user.CustomerId.HasValue ?  _customers
                    .First(x => x.Id == user.CustomerId) : null;

                return new UserView()
                {
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    CustomerDisplay = customer is not null ? 
                        $"{customer.Number} - {customer.Name}" : null
                };
            });

            _userViews = userViews;
        }

        private async Task FetchRoles()
        {
            var roles = await RoleManager.Roles
                .AsNoTracking()
                .Select(x => x.Name)
                .ToListAsync();
            _roles = roles;
        }

        private async Task FetchCustomers()
        {
            _customers = await CustomerQueries.List();
        }

        private string GetRoleDisplayName(string role)
        {
            if(role == UserTypes.ADMINISTRATOR)
            {
                return "Administrator";
            }
            else if(role == UserTypes.CUSTOMER)
            {
                return "Kunde";
            }

            return "Udefinert rolle";
        }

        private async Task HandleValidSubmit()
        {
            if (NewUser.Password != NewUser.RepeatPassword)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Passord må være like..");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.UserName))
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Brukernavn må være oppgitt..");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.Email))
            {
                NotificationService.Notify(NotificationSeverity.Warning, "E-post må være oppgitt..");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.Role))
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Rolle må være oppgitt..");
                return;
            }

            var user = new ApplicationUser 
            { 
                UserName = NewUser.UserName,
                Email = NewUser.Email,
                CustomerId = NewUser.CustomerId,
            };

            var result = await UserManager
                .CreateAsync(user, NewUser.Password);

            if (result.Succeeded)
            {
                var emailConfirmationCode = await UserManager
                    .GenerateEmailConfirmationTokenAsync(user);
                var confirmEmailRes = await UserManager
                    .ConfirmEmailAsync(user, emailConfirmationCode);

                await UserManager
                    .AddToRoleAsync(user, NewUser.Role);
            }

            NewUser = new();
            await FetchUsers();
        }

        private class NewUserModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string RepeatPassword { get; set; }

            [Required]
            public string Role { get; set; }

            [Required]
            public Guid CustomerId { get; set; }
        }

        private class UserView : ApplicationUser
        {
            public string? CustomerDisplay { get; set; }
        }
    }
}
