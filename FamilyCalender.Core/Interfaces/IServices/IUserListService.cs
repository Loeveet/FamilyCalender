using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IServices
{
	public interface IUserListService
	{
		Task<List<UserList>> GetListsForUserAsync(int userId);
		Task<List<UserList>> GetListsForUserByCalendarAsync(int userId, int calendarId);
        Task CreateListAsync(int userId, string listName, int? calendarId);
		Task<UserList?> GetListByIdAsync(int listId, int userId);
		Task <List<ListItem>> GetItemsForListAsync(int listId);
		Task AddItemToListAsync(int userId, int listId, string name);
		Task DeleteListAsync(int userListId);
        Task<ListItem?> GetItemByIdAsync(int listItemId);
        Task UpdateItemAsync(ListItem item);
        Task DeleteItemAsync(ListItem item);
    }
}
