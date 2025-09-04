using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
namespace FamilyCalender.Web.Pages
{
	[ValidateAntiForgeryToken]

	public class ListPageModel(
		IAuthService authService,
		IUserListService userListService,
		EncryptionService encryptionService) : BasePageModel(authService)
	{
		private readonly IUserListService _userListService = userListService;
		private readonly EncryptionService _encryptionService = encryptionService;

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
		public class UpdateListOrderDto
		{
			public int CalendarId { get; set; }
			public List<int> SortedIds { get; set; } = new();
		}
		public class UpdateListDto
		{
			public int Id { get; set; }
			public string Name { get; set; } = string.Empty;
		}
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
		public async Task<IActionResult> OnPostUpdateListNameAsync([FromBody] UpdateListDto data)
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == null)
				return Unauthorized();

			if (data == null || string.IsNullOrWhiteSpace(data.Name))
				return BadRequest(new { success = false, message = "Ogiltigt data" });

			// Hämta listan via service
			var list = await _userListService.GetListByIdAsync(data.Id, userId.Value);
			if (list == null)
				return NotFound(new { success = false, message = "Hittade inte listan" });

			var encryptedItemName = _encryptionService.AutoDetectEncryptStringToString(data.Name, list.NameIv.ToString());

			var originalName = list.Name;
			list.Name = encryptedItemName;
			list.LastEditedUtc = DateTime.UtcNow;

			try
			{
				await _userListService.UpdateListAsync(list);
				return new JsonResult(new { success = true });
			}
			catch (Exception ex)
			{
				return new JsonResult(new { success = false, message = ex.Message, originalName });
			}
		}
		public async Task<IActionResult> OnPostUpdateListOrderAsync([FromBody] UpdateListOrderDto data)
		{
			var user = await GetCurrentUserAsync();
			if (user == null)
				return Unauthorized();

			if (data == null || data.SortedIds == null || !data.SortedIds.Any())
				return BadRequest();

			// Hämta alla listor för användaren (eller filtrera på kalender)
			var allLists = data.CalendarId > 0
				? await _userListService.GetListsForUserByCalendarAsync(user.Id, data.CalendarId)
				: await _userListService.GetListsForUserAsync(user.Id);

			// Filtrera bara de listor som ska uppdateras
			var listsToUpdate = allLists.Where(l => data.SortedIds.Contains(l.Id)).ToList();

			for (int i = 0; i < data.SortedIds.Count; i++)
			{
				var list = listsToUpdate.FirstOrDefault(l => l.Id == data.SortedIds[i]);
				if (list != null)
				{
					list.SortOrder = i + 1;
				}
			}

			// Spara via service
			await _userListService.UpdateListsAsync(listsToUpdate);

			return new JsonResult(new { success = true });
		}
	}
}
