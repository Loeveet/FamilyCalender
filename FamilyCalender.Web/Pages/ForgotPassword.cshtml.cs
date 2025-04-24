using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyCalender.Core.Interfaces.IServices;

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
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError("", "Ange en giltig e-postadress.");
            }

            await _authService.SendPasswordResetEmailAsync(Email);
            Message = "Om e-postadressen finns registrerad har vi skickat en återställningslänk som är giltlig en timme.";
            return Page();
        }
    }
}
