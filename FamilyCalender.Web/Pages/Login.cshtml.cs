using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FamilyCalender.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
		[DataType(DataType.Password)]
		public string Password { get; set; }

        public string ErrorMessage { get; set; }

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
