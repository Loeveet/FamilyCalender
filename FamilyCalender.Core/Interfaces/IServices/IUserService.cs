using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task CreateNotificationAsync(UserNotificationSetting uns);
        Task UpdateNotificationAsync(UserNotificationSetting uns);
        Task DeleteNotificationAsync(int userId);
        Task<int?> GetPreferredCalendarIdAsync(int userId);
        Task UpdatePreferredCalendarIdAsync(int userId, int preferredCalenderId);
    }
}
