using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Web.Code;

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
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
	            Log.Debug($"User {Email} already logged in - redirected to /CalendarOverview");
				return RedirectToPage("/CalendarOverview");
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
	        Log.Debug($"User tries to login {Email}");
			if (await _authService.IsValidUserNamePassword(Email, Password))
            {
	            Log.Debug($"User {Email} successfully logged in ");
				var claims = new List<Claim>
	            {
		            new Claim(ClaimTypes.Name, Email)
	            };
	            var identity = new ClaimsIdentity(claims, GlobalSettings.AuthCookieName);
	            var principal = new ClaimsPrincipal(identity);

	            var authProperties = new AuthenticationProperties
	            {
		            IsPersistent = true,
		            ExpiresUtc = DateTime.UtcNow.AddDays(365)
	            };

	            await HttpContext.SignInAsync(GlobalSettings.AuthCookieName, principal, authProperties);

				string returnUrl = HttpContext.Request.Query["returnUrl"].ToString() ?? "";
                if (Url.IsLocalUrl(returnUrl))
                {
	                Log.Debug($"User {Email} came with redirect url {returnUrl} ");
					return Redirect(returnUrl);
                }
              
                return RedirectToPage("/CalendarOverview");
            }

            Log.Information($"Invalid credentials for user {Email}");

			ErrorMessage = "Invalid login attempt.";
            return Page();
        }
    }
}
