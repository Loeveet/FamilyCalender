using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly IAuthService _authService;

		public LogoutModel(IAuthService authService)
		{
			_authService = authService;
		}

		public async Task<IActionResult> OnPost()
		{
			await _authService.LogoutAsync();
            return Redirect("/");
        }
	}
}
