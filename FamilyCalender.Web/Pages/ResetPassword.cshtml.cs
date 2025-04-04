using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FamilyCalender.Web.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IAuthService _authService;

        public ResetPasswordModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Token { get; set; }
        [BindProperty]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [BindProperty]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }


        public IActionResult OnGet(string token)
        {
            Token = token;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword.Length < 12)
            {
                ErrorMessage = "L�senorden m�ste vara minst 12 tecken.";
                return Page();
            }
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "L�senorden matchar inte.";
                return Page();
            }

            var result = await _authService.ResetPasswordAsync(Token, NewPassword);
            if (!result)
            {
                ErrorMessage = "Ogiltig eller utg�ngen �terst�llningsl�nk.";
                return Page();
            }
            SuccessMessage = "Ditt l�senord �r �ndrat. Logga in igen.";
            return Page();
        }
    }
}
