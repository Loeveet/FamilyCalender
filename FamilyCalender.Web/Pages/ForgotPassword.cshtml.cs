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
        [Required(ErrorMessage = "E-postadress �r obligatorisk.")]
        [EmailAddress(ErrorMessage = "E-postadressen �r ogiltig.")]

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
                return Page(); // stanna h�r och visa valideringsfel
            }

            await _authService.SendPasswordResetEmailAsync(Email);
            Message = "Om e-postadressen finns registrerad har vi skickat en �terst�llningsl�nk som �r giltlig en timme. Kolla skr�pposten om du inte hittar n�got utskick.";
            return Page();
        }
    }
}
