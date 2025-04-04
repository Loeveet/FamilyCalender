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
		private readonly IEmailService _emailService;


		public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_emailService = emailService;
		}
		public async Task<bool> LoginAsync(string email, string password)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return false;

			var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
			if (!isValidPassword)
				return false;

			var isVerified = user.IsVerified;
			if (!isVerified)
			{
				return false;
			}

			var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Email)
		};
			var identity = new ClaimsIdentity(claims, "Cookie");
			var principal = new ClaimsPrincipal(identity);

			var authProperties = new AuthenticationProperties
			{
				IsPersistent = true,
				//ExpiresUtc = DateTime.UtcNow.AddDays(365)
			};

			if (_httpContextAccessor.HttpContext != null)
			{
				await _httpContextAccessor.HttpContext.SignInAsync("Cookie", principal, authProperties);
			}

			return true;
		}

		public async Task<(bool Succeeded, string Error, string Token)> RegisterAsync(string email, string password)
		{
			var userExists = await _context.Users.AnyAsync(u => u.Email == email);
			if (userExists)
			{
				return (false, "E-postadressen används redan.", string.Empty);
			}

			try
			{
				var user = new User
				{
					Email = email,
					PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
					IsVerified = false,
					VerificationToken = Guid.NewGuid().ToString(),

				};

				await _context.Users.AddAsync(user);
				await _context.SaveChangesAsync();

				_emailService.SendVerificationEmail(user.Email, user.VerificationToken);

				return (true, string.Empty, user.VerificationToken);
			}
			catch (Exception ex)
			{

				return (false, "Ett fel uppstod under registrering. Vänligen försök igen senare.", string.Empty);
			}
		}



		public async Task LogoutAsync()
		{
			if (_httpContextAccessor.HttpContext != null)
			{
				await _httpContextAccessor.HttpContext.SignOutAsync("Cookie");
			}
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			//Gör en try catch eller returna ett fel om användaren inte hittas
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
		public async Task<User> GetUserByTokenAsync(string verificationToken)
		{
			//Gör en try catch eller returna ett fel om användaren inte hittas
			return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);
		}
        public async Task<User> GetUserByPasswordResetTokenAsync(string passwordResetToken)
        {
            //Gör en try catch eller returna ett fel om användaren inte hittas
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == passwordResetToken);
        }

        public async Task<bool> VerifyAccount(string token)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
			if (user == null)
			{
				return false;
			}

			SetUserToVerified(user);
			await _context.SaveChangesAsync();

			return true;
		}

		private static void SetUserToVerified(User user)
		{
			user.IsVerified = true;
			user.VerificationToken = null;
			user.VerificationDateUtc = DateTime.UtcNow;
		}

		public async Task SendPasswordResetEmailAsync(string email)
		{
			var user = await GetUserByEmailAsync(email);
			if (user == null) return;

			var token = Guid.NewGuid().ToString();
			user.PasswordResetToken = token;
			user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
			await _context.SaveChangesAsync();

			_emailService.SendPasswordResetEmail(user.Email, token);
		}

        public async Task<bool> ResetPasswordAsync(string resetToken, string newPassword)
        {
            var user = await GetUserByPasswordResetTokenAsync(resetToken);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return false; // Token är ogiltig eller har gått ut
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
			await _context.SaveChangesAsync();
            return true;
        }
    }
}
