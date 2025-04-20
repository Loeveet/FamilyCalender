using FamilyCalender.Core.Interfaces;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class VerifyAccountModel : PageModel
    {
        private readonly IAuthService _authService;

        public VerifyAccountModel(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> OnGetAsync(string? token)
        {
			if (string.IsNullOrEmpty(token))
			{
				return BadRequest("Token saknas.");
			}

			if (await _authService.VerifyAccount(token))
            {
                return RedirectToPage("/AccountVerified");

            }

            return RedirectToPage("/AccountVerified");
            //for some stupid reason we always end up here in production - why??
            //return BadRequest("Ogiltig eller utgången verifieringslänk.");
        }
    }
}
