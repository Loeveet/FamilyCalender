using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace FamilyCalender.Web.Pages
{
    public class LoginModel(IAuthService authService) : BasePageModel(authService)
    {

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            Log.Information("Testing login page");
            
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                return RedirectToPage("/Index");
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _authService.LoginAsync(Email, Password))
            {
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid login attempt.";
            return Page();
        }
    }
}
