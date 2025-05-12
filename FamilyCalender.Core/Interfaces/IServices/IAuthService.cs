using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IAuthService
    {
		Task<bool> IsValidUserNamePassword(string email, string password);
		Task<(bool Succeeded, string Error, string Token)> RegisterAsync(string email, string password);
		Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByTokenAsync(string verificationToken);
        Task<User> GetUserByPasswordResetTokenAsync(string passwordResetToken);
        Task<User> GetUserByIdAsync(int ownerId);
		Task<bool> VerifyAccount(string token);
        Task SendPasswordResetEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task SetLastLoggedInAsync(User user);


    }
}
