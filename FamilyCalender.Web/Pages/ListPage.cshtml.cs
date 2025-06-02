using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
namespace FamilyCalender.Web.Pages
{
    public class ListPageModel(
		IAuthService authService,
		IUserListService userListService) : BasePageModel(authService)
	{
		private readonly IUserListService _userListService = userListService;

		[BindProperty(SupportsGet = true)]
		public int? CalendarId { get; set; }
		[BindProperty(SupportsGet = true)]
		public string? CalendarName { get; set; }
        [BindProperty]
        public int UserListId { get; set; }
        [BindProperty]
        public string? NewListName { get; set; }
		[BindProperty]
		public ListTypeEnum NewListType { get; set; } = ListTypeEnum.Unknown;


		public List<UserList> Lists { get; set; } = new();
		public async Task<IActionResult> OnGetAsync()
        {
			var user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToPage("/Login");
			}

			if (CalendarId.HasValue)
			{
				Lists = await _userListService.GetListsForUserByCalendarAsync(user.Id, CalendarId.Value);
			}
			else
			{
				Lists = await _userListService.GetListsForUserAsync(user.Id);
			}

			return Page();
		}
        public async Task<IActionResult> OnPostDeleteListAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
                return RedirectToPage("/Login");

            if (UserListId == 0)
            {
                ModelState.AddModelError("", "Ogiltigt list-id.");
                return Page();
            }

            await _userListService.DeleteListAsync(UserListId);

            return RedirectToPage(new { CalendarId , CalendarName});
        }
        public async Task<IActionResult> OnPostCreateNewListAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
                return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(NewListName))
            {
				ModelState.AddModelError("NewListName", "Namnet på listan kan inte vara tomt.");
                return Page();
            }

            if (NewListType == ListTypeEnum.Unknown)
            {
				ModelState.AddModelError("NewListType", "Du måste välja typ av lista.");
				return Page();
			}

            await _userListService.CreateListAsync(userId.Value, NewListName, CalendarId, NewListType);
            return RedirectToPage(new { calendarId = CalendarId, calendarName = CalendarName });
        }

    }
}
