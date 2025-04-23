using FamilyCalender.Web.Code;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class LogoutModel : PageModel
    {
		public async Task<IActionResult> OnPost()
		{
			await HttpContext.SignOutAsync(GlobalSettings.AuthCookieName);
			
			return Redirect("/");
        }
	}
}
