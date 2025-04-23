using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Infrastructure.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task CreateNotificationAsync(UserNotificationSetting uns)
        {
            await _userRepository.CreateNotificationAsync(uns);
        }

        public async Task UpdateNotificationAsync(UserNotificationSetting uns)
        {
            await _userRepository.UpdateNotificationAsync(uns);
        }

        public async Task<UserNotificationSetting> GetNotificationSetting(int userId)
        {
            return await _userRepository.GetNotificationSetting(userId);
        }

        public async Task DeleteNotificationAsync(int userId)
        {
            await _userRepository.DeleteNotificationAsync(userId);

		}


	}
}
