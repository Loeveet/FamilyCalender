using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyCalender.Web.Pages
{
    [Authorize]
    public class UserSettingModel(IUserService userService, IAuthService authService) : BasePageModel(authService)
    {

        [BindProperty]
        public NotificationSettings NotificationSettings { get; set; } = new NotificationSettings();


		private static readonly string[] AllowedEmails = new[]
        {
            "loeveet@gmail.com",
            "mikael.lennander@hotmail.com"
        };

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || !AllowedEmails.Contains(user.Email))
            {
                return RedirectToPage("/CalendarOverview");
            }

            NotificationSettings = new NotificationSettings()
            {
                UserNotificationSetting = user.NotificationSetting ?? new UserNotificationSetting
                {
                    UserId = user.Id,
                },
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!NotificationSettings.UserNotificationSetting.AllowNotifications)
            {
                NotificationSettings.HasTurndOff = true;
				await userService.DeleteNotificationAsync(NotificationSettings.UserNotificationSetting.UserId);
			}
			else
            {
				await userService.UpdateNotificationAsync(NotificationSettings.UserNotificationSetting);
			}
             
			return Page();
		}


	}

    public class NotificationSettings
    {
        public UserNotificationSetting UserNotificationSetting { get; set; }
        public bool IsRegistered => !string.IsNullOrEmpty(UserNotificationSetting?.Endpoint);

		public bool HasTurndOff { get; set; }
	}
}
