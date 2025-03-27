using FamilyCalender.Core.Models.Entities;
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
		Task<(bool Succeeded, string Error)> RegisterAsync(string email, string password);
		Task LogoutAsync();
		Task<User> GetUserByEmailAsync(string email);
	}
}
