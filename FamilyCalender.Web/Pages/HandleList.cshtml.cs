using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyCalender.Web.Pages;
//[IgnoreAntiforgeryToken]
[ValidateAntiForgeryToken]


public class HandleListModel(IUserListService userListService, IAuthService authService, ApplicationDbContext context) : BasePageModel(authService)
{
    private readonly IUserListService _userListService = userListService;
	private readonly ApplicationDbContext _context = context;

	[BindProperty(SupportsGet = true)]
    public int ListId { get; set; }
    [BindProperty(SupportsGet = true)]
    public string? CalendarName { get; set; }

    public UserList? UserList { get; set; }

    public List<ListItem> Items { get; set; } = [];

    [BindProperty]
    public string NewItemName { get; set; } = string.Empty;
    public class ToggleItemDto
    {
        public int ItemId { get; set; }
    }
    public record DeleteItemDto(int ItemId);
	public class UpdateOrderDto
	{
		public int ListId { get; set; }
		public List<int> SortedIds { get; set; }
	}


	public async Task<IActionResult> OnGetAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
			return RedirectToPage("/Login");

		UserList = await _userListService.GetListByIdAsync(ListId, user.Id);
        if (UserList is null)
            return NotFound();

        Items = await _userListService.GetItemsForListAsync(ListId);
        return Page();
    }

    public async Task<IActionResult> OnPostAddItemAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId == null)
			return RedirectToPage("/Login");
        
		await _userListService.AddItemToListAsync(userId.Value, ListId, NewItemName);
        return RedirectToPage(new { listId = ListId, CalendarName });
    }

    public async Task<IActionResult> OnPostToggleItemAsync([FromBody] ToggleItemDto data)
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId == null)
            return Unauthorized();

        var item = await _userListService.GetItemByIdAsync(data.ItemId);
        if (item == null)
            return NotFound();

        item.State = item.State == ListItemTypeEnum.Checked ? ListItemTypeEnum.Undefined : ListItemTypeEnum.Checked;
        item.StateUpdatedUtc = DateTime.UtcNow;
        item.UpdateUserId = userId.Value;
        item.LastEditedUtc = DateTime.UtcNow;

		await _userListService.UpdateItemAsync(item);

        return new JsonResult(new { success = true, newState = item.State.ToString() });
    }


    public async Task<IActionResult> OnPostDeleteItemAsync([FromBody] DeleteItemDto data)
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId == null)
            return Unauthorized();

        var item = await _userListService.GetItemByIdAsync(data.ItemId);
        if (item == null)
            return NotFound();

        await _userListService.DeleteItemAsync(item);

        return new JsonResult(new { success = true });
    }
	public async Task<IActionResult> OnPostUpdateOrderAsync([FromBody] UpdateOrderDto data)
	{
		var userId = await GetCurrentUserIdAsync();
		if (userId == null)
			return Unauthorized();

		if (data == null || data.SortedIds == null)
			return BadRequest();

		var userList = await _userListService.GetListByIdAsync(data.ListId, userId.Value);
		if (userList == null)
			return NotFound();

		for (int i = 0; i < data.SortedIds.Count; i++)
		{
			var item = await _context.ListItems.FindAsync(data.SortedIds[i]);
			if (item != null)
			{
				item.SortOrder = data.SortedIds.Count - i;
			}
		}

		await _context.SaveChangesAsync();
		return new JsonResult(new { success = true });
	}

}
