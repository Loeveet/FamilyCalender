using FamilyCalender.Core.Interfaces;
using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FamilyCalender.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;

        public RegisterModel(IAuthService authService)
        {
            _authService = authService;

        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
		public string ErrorMessage { get; set; }


		public class InputModel
        {

            [Required]
            [EmailAddress]
            [Display(Name = "E-post")]
            public string Email { get; set; }

            [Required]
            //S�tt krav p� 10-12 tecken samt specialtecken n�r det �r dags att k�ra skarpt sen
            //[StringLength(50, ErrorMessage = "L�senordet m�ste vara minst {2} och max {1} tecken l�ngt.", MinimumLength = 12)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Bekr�fta l�senord")]
            [Compare("Password", ErrorMessage = "L�senordet och det bekr�ftade l�senordet st�mmer inte.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var (Succeeded, Error) = await _authService.RegisterAsync(Input.Email, Input.Password);

                if (Succeeded)
                {
                    return RedirectToPage("/Login");
                }
                ErrorMessage = Error;

				foreach (var error in Error)
                {
                    ModelState.AddModelError(string.Empty, error.ToString());
                }
            }

            return Page();
        }

    }
}
