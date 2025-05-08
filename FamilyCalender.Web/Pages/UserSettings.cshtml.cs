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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
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
            var user = await GetCurrentUserAsync();
            if (!NotificationSettings.UserNotificationSetting.AllowNotifications)
            {
                NotificationSettings.HasTurndOff = true;
				await userService.DeleteNotificationAsync(NotificationSettings.UserNotificationSetting.UserId);
			}
			else
            {
				await userService.UpdateNotificationAsync(NotificationSettings.UserNotificationSetting);
			}

            NotificationSettings.SavedSuccess = true;
            return Page();
			//return RedirectToPage("./UserSettings"); //hm, tanken är ju att komma till turn off så det avreggas, men så blir väl inte fallet nu?
		}


	}

    public class NotificationSettings
    {
        public UserNotificationSetting UserNotificationSetting { get; set; }
        public bool IsRegistered => !string.IsNullOrEmpty(UserNotificationSetting?.Endpoint);

		public bool HasTurndOff { get; set; }
		public bool SavedSuccess { get; internal set; }
	}
}
