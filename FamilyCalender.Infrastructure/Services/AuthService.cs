using FamilyCalender.Core.Interfaces;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace FamilyCalender.Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;


		public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<bool> LoginAsync(string email, string password)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return false;

			// Sätt användarens login status, till exempel genom att använda sessions
			var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
			if (!isValidPassword)
				return false;

			var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Email)
		};
			var identity = new ClaimsIdentity(claims, "Cookie");
			var principal = new ClaimsPrincipal(identity);

			if (_httpContextAccessor.HttpContext != null)
			{
				await _httpContextAccessor.HttpContext.SignInAsync("Cookie", principal);
			}

			return true;
		}

        public async Task<(bool Succeeded, string Error)> RegisterAsync(string email, string password)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == email);
            if (userExists)
            {
				return (false, "E-postadressen används redan.");
			}

			try
            {
                var user = new User
                {
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {

				return (false, "Ett fel uppstod under registrering. Vänligen försök igen senare.");
			}
		}



        public async Task LogoutAsync()
		{
			if (_httpContextAccessor.HttpContext != null)
			{
				await _httpContextAccessor.HttpContext.SignOutAsync();
			}
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			//Gör en try catch eller returna ett fel om användaren inte hittas
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
	}
}
