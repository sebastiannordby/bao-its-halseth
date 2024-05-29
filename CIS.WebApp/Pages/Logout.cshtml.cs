using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace CIS.WebApp.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet(string redirectUri)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Redirect(redirectUri ?? "/");
        }
    }
}
