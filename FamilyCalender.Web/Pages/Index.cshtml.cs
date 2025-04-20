using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class IndexModel() : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
        {

	        if (HttpContext.User.Identity.IsAuthenticated)
	        {
				// so fucking ugly due to .net 8 asp 
		        return Redirect("/CalendarOverview");
	        }
			return Page();
		}
	}
}
