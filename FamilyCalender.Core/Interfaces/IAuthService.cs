using FamilyCalender.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces
{
    public interface IAuthService
    {
		Task<bool> LoginAsync(string email, string password);
		Task<(bool Succeeded, string Error, string Token)> RegisterAsync(string email, string password);
		Task LogoutAsync();
		Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByTokenAsync(string verificationToken);
        Task<bool> VerifyAccount(string token);
    }
}
