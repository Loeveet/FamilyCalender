using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Core.Models.ViewModels;
using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class IndexModel() : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
        {
           
			return Page();
		}
	}
}
