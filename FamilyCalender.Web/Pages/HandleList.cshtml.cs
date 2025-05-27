using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages;
[IgnoreAntiforgeryToken]

public class HandleListModel(IUserListService userListService, IAuthService authService) : BasePageModel(authService)
{
    private readonly IUserListService _userListService = userListService;

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

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId == null)
			return RedirectToPage("/Login");

		UserList = await _userListService.GetListByIdAsync(ListId, userId.Value);
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
        return RedirectToPage(new { listId = ListId });
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


}
