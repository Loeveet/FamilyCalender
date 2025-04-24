using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class SuperAdminModel(IUserService userService, ICalendarService calendarService, IAuthService authService) : BasePageModel(authService)
    {
        private readonly IUserService _userService = userService;
        private readonly ICalendarService _calendarService = calendarService;

        public bool IsAdmin { get; set; }
        public List<User> Users { get; set; } = [];
        public int VerifiedUsers { get; set; }
        public List<Calendar> Calendars { get; set; } = [];
        public DateTime? LatestCalendarUpdate { get; set; }
        public List<CalendarDto> InactiveCalendars { get; set; } = new();

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
                return RedirectToPage("/Index");
            }
            IsAdmin = true;

            Users = await _userService.GetAllUsersAsync();
            Calendars = await _calendarService.GetAllCalendarsAsync();

            VerifiedUsers = Users.Where(u => u.IsVerified).Count();
            return Page();
        }

    }
}
