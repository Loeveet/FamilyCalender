using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IAuthService _authService;

        public ForgotPasswordModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError("", "Ange en giltig e-postadress.");
                return Page();
            }

            await _authService.SendPasswordResetEmailAsync(Email);
            TempData["Message"] = "Om e-postadressen finns registrerad har vi skickat en återställningslänk.";
            return RedirectToPage("/Login");
        }
    }
}
