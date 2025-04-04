using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public string NewPassword { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet(string token)
        {
            Token = token;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                Message = "Lösenorden matchar inte.";
                return Page();
            }

            var result = await _authService.ResetPasswordAsync(Token, NewPassword);
            if (!result)
            {
                Message = "Ogiltig eller utgången återställningslänk.";
                return Page();
            }

            return RedirectToPage("/Login");
        }
    }
}
