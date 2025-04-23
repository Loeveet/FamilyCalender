using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<UserNotificationSetting> GetNotificationSetting(int userId);

        Task CreateNotificationAsync(UserNotificationSetting uns);
        Task UpdateNotificationAsync(UserNotificationSetting uns);
        Task DeleteNotificationAsync(int userId);

	}
}
