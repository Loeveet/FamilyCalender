using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyCalender.Core.Interfaces.IServices;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "E-postadress är obligatorisk.")]
        [EmailAddress(ErrorMessage = "E-postadressen är ogiltig.")]

        public string Email { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError(nameof(Email), "Ange en giltig e-postadress.");
            }
            if (!ModelState.IsValid)
            {
                return Page(); // stanna här och visa valideringsfel
            }

            await _authService.SendPasswordResetEmailAsync(Email);
            Message = "Om e-postadressen finns registrerad har vi skickat en återställningslänk som är giltlig en timme. Kolla skräpposten om du inte hittar något utskick.";
            return Page();
        }
    }
}
