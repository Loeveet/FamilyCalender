using FamilyCalender.Core.Interfaces;
using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FamilyCalender.Web.Pages
{
	public class BasePageModel : PageModel
	{
		protected readonly IAuthService _authService;

		public BasePageModel(IAuthService authService)
		{
			_authService = authService;
		}

		// Kolla om användaren är inloggad
		protected async Task<User> GetCurrentUserAsync()
		{
			var userEmail = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;
			if (string.IsNullOrEmpty(userEmail))
			{
				return null;
			}

			var user = await _authService.GetUserByEmailAsync(userEmail);
			return user;
		}
	}
}
