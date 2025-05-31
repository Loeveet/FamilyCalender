using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class UserListService(ApplicationDbContext context, EncryptionService encryptionService) : IUserListService
    {
        private readonly ApplicationDbContext _context = context;
		private readonly EncryptionService _encryptionService = encryptionService;

		public async Task CreateListAsync(int userId, string listName, int? calendarId, ListTypeEnum type)
        {
            if (string.IsNullOrWhiteSpace(listName))
                throw new ArgumentException("Listnamnet får inte vara tomt.", nameof(listName));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
				var iv = RandomNumberGenerator.GetBytes(16);
                var encryptedName = _encryptionService.AutoDetectEncryptStringToString(listName, iv.ToString());

				var newList = new UserList
                {
                    Name = encryptedName,
                    NameIv = iv,
					OwnerId = userId,
                    CalendarId = calendarId,
                    CreatedUtc = DateTime.UtcNow,
                    Type = type
                };

                _context.UserLists.Add(newList);
                await _context.SaveChangesAsync();

                if (calendarId.HasValue)
                {
                    var calendarMembers = await _context.CalendarAccesses
                        .Where(ca => ca.CalendarId == calendarId.Value)
                        .Select(ca => ca.UserId)
                        .ToListAsync();

                    foreach (var memberId in calendarMembers)
                    {
                        _context.ListAccesses.Add(new ListAccess
                        {
                            UserId = memberId,
                            UserListId = newList.Id,
                            IsOwner = memberId == userId,
                            JoinedUtc = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    _context.ListAccesses.Add(new ListAccess
                    {
                        UserId = userId,
                        UserListId = newList.Id,
                        IsOwner = true,
                        JoinedUtc = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

            }
        }
        public async Task AddItemToListAsync(int userId, int userListId, string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                throw new ArgumentException("Item-namn får inte vara tomt.", nameof(itemName));


            var list = await _context.UserLists.FindAsync(userListId);
            if (list == null)
                throw new InvalidOperationException("Listan kunde inte hittas.");

            var hasAccess = await _context.ListAccesses
                .AnyAsync(la => la.UserId == userId && la.UserListId == userListId);
            if (!hasAccess)
                throw new UnauthorizedAccessException("Användaren har inte åtkomst till listan.");

            var currentMaxSortOrder = await _context.ListItems
                .Where(li => li.UserListId == userListId)
                .MaxAsync(li => (int?)li.SortOrder) ?? 0;

            var encryptedItemName = _encryptionService.AutoDetectEncryptStringToString(itemName, userListId.ToString());

            var newItem = new ListItem
            {
                Name = encryptedItemName,
                UserListId = userListId,
                CreatedUserId = userId,
                CreatedUtc = DateTime.UtcNow,
                State = ListItemTypeEnum.Undefined,
                SortOrder = currentMaxSortOrder + 1
            };

            _context.ListItems.Add(newItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserList>> GetListsForUserAsync(int userId)
        {
            var lists = await _context.ListAccesses
                .Where(la => la.UserId == userId)
                .Include(la => la.UserList)
                    .ThenInclude(l => l!.Owner)
                .Include(la => la.UserList)
                    .ThenInclude(l => l!.Items)
                .Select(la => la.UserList!)
                .ToListAsync();

            return lists;
        }
        public async Task<List<UserList>> GetListsForUserByCalendarAsync(int userId, int calendarId)
        {
            var lists = await _context.ListAccesses
                .Where(la => la.UserId == userId && la.UserList!.CalendarId == calendarId)
                .Include(la => la.UserList)
                    .ThenInclude(l => l!.Owner)
                .Include(la => la.UserList)
                    .ThenInclude(l => l!.Items)
                .Select(la => la.UserList!)
                .ToListAsync();

            if(lists.Count > 0)
            {
                foreach (var list in lists)
                {
                    list.Name = _encryptionService.AutoDetectDecryptStringToString(list.Name, list.NameIv.ToString());
                }
            }

            return lists;
        }

        public async Task<UserList?> GetListByIdAsync(int listId, int userId)
        {
            var userList = await _context.UserLists
                .Include(ul => ul.Owner)
                .Where(ul => ul.Id == listId
                        && _context.ListAccesses.Any(la => la.UserListId == ul.Id && la.UserId == userId))
                .FirstOrDefaultAsync();

            if(userList != null)
            {
				userList.Name = _encryptionService.AutoDetectDecryptStringToString(userList.Name, userList.NameIv.ToString());
			}

			return userList;
        }


        public async Task<List<ListItem>> GetItemsForListAsync(int listId)
        {
            var listOfItems = await _context.ListItems
                .Where(item => item.UserListId == listId)
                .OrderBy(item => item.SortOrder)
                .ToListAsync();

			if (listOfItems.Count > 0)
			{
				foreach (var items in listOfItems)
				{
					items.Name = _encryptionService.AutoDetectDecryptStringToString(items.Name, items.UserListId.ToString());
				}
			}

			return listOfItems;
		}

        public async Task DeleteListAsync(int userListId)
        {
            var items = _context.ListItems.Where(i => i.UserListId == userListId);
            var list = await _context.UserLists.FindAsync(userListId);
            if (list == null)
                throw new InvalidOperationException("Listan kunde inte hittas.");
            _context.ListItems.RemoveRange(items);
            _context.UserLists.Remove(list);
            await _context.SaveChangesAsync();
        }

        public async Task<ListItem?> GetItemByIdAsync(int listItemId)
        {
            var listItem = await _context.ListItems
                .Include(i => i.UserList)
                .FirstOrDefaultAsync(i => i.Id == listItemId);

            return listItem;
        }

        public async Task UpdateItemAsync(ListItem item)
        {
            _context.ListItems.Update(item);

			var userList = await _context.UserLists.FirstOrDefaultAsync(ul => ul.Id == item.UserListId);
			if (userList != null)
			{
				userList.LastEditedUtc = DateTime.UtcNow;
			}
			await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(ListItem item)
        {
            _context.ListItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

}
