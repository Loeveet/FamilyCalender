using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using FamilyCalender.Core.Interfaces.IServices;
using Serilog;

namespace FamilyCalender.Web.Pages
{
	public class BasePageModel : PageModel
	{
		protected readonly IAuthService _authService;

		public BasePageModel(IAuthService authService)
		{
			_authService = authService;
		}

		protected async Task<User> GetCurrentUserAsync()
		{
			try
			{
				var userEmail = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;
				if (string.IsNullOrEmpty(userEmail))
				{
					return null;
				}

				var user = await _authService.GetUserByEmailAsync(userEmail);
				return user;
			}
			catch (Exception e)
			{
				Log.Error($"Error GetCurrentUserAsync due to {e.Message}");
			}

			return null;
		}
	}
}
