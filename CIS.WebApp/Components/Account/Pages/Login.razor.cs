using CIS.Application.Features.Shared;

namespace CIS.WebApp.Components.Account.Pages
{
    public partial class Login
    {
        public void FillInAdminLogin()
        {
            Input.Email = DemoConstants.ADMIN_USERNAME;
            Input.Password = DemoConstants.ADMIN_PASSWORD;
            StateHasChanged();
        }

        public void FillInCustomerLogin()
        {
            Input.Email = DemoConstants.CUSTOMER_USERNAME;
            Input.Password = DemoConstants.ADMIN_PASSWORD;
            StateHasChanged();
        }
    }
}
