using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using FamilyCalender.Core.Interfaces.IServices;
using Serilog;
using FamilyCalender.Web.Code;

namespace FamilyCalender.Web.Pages
{
	public class BasePageModel : PageModel
	{
		protected readonly IAuthService _authService;

		public BasePageModel(IAuthService authService)
		{
			_authService = authService;
		}

        protected readonly string[] AllowedEmails = new[]
        {
            "loeveet@gmail.com",
            "mikael.lennander@hotmail.com",
            "carolinaguevara@hotmail.com",
            "jenny.liliegren@outlook.com",
			"demokonto86@gmail.com",
		};

        protected readonly string[] SuperAdminEmails = new[]
        {
            "loeveet@gmail.com",
            "mikael.lennander@hotmail.com",
        };

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

                ViewData[GlobalSettings.ShowBetaTesterMenu] = AllowedEmails.Contains(user.Email);
                ViewData[GlobalSettings.ShowSuperAdminMenu] = SuperAdminEmails.Contains(user.Email);
                ViewData[GlobalSettings.ShowCalendarMenu] = true;
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
