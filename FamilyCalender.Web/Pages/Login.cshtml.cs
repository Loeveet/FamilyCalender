using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace FamilyCalender.Web.Pages
{
    public class LoginModel(IAuthService authService) : BasePageModel(authService)
    {

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            Log.Information("Testing login page");
            
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                return RedirectToPage("/CalendarOverview");
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _authService.LoginAsync(Email, Password))
            {
                string returnUrl = HttpContext.Request.Query["returnUrl"];
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
              
                return RedirectToPage("/CalendarOverview");
            }

            ErrorMessage = "Invalid login attempt.";
            return Page();
        }
    }
}
