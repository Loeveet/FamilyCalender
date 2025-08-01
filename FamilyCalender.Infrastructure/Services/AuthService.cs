﻿using FamilyCalender.Core.Interfaces;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Services
{
	public class AuthService(ApplicationDbContext context, IEmailService emailService) : IAuthService
	{
		private readonly ApplicationDbContext _context = context;
		private readonly IEmailService _emailService = emailService;

		public async Task<bool> IsValidUserNamePassword(string email, string password)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				return false;
			}

			var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
			if (!isValidPassword)
			{
				return false;
			}

			var isVerified = user.IsVerified;
			if (!isVerified)
			{
				return false;
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

				_emailService.SendVerificationEmail(user.Email, user.VerificationToken);
				await _context.SaveChangesAsync();

				return (true, string.Empty, user.VerificationToken);
			}
			catch (Exception ex)
			{

				return (false, "Ett fel uppstod under registrering. Vänligen försök igen senare.", string.Empty);
			}
		}


        public async Task<User?> GetUserByEmailForPasswordAsync(string email)
        {
            var user = await _context.Users
                .Include(c => c.NotificationSetting)
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }


        public async Task<User> GetUserByEmailAsync(string email)
		{
			//Gör en try catch eller returna ett fel om användaren inte hittas
			var user = await _context.Users
				.Include(c => c.NotificationSetting)
				.FirstOrDefaultAsync(u => u.Email == email);
			return user;
        }
		public async Task<User> GetUserByTokenAsync(string verificationToken)
		{
			//Gör en try catch eller returna ett fel om användaren inte hittas
			return await _context.Users
                .Include(c => c.NotificationSetting)
                .FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);
		}
        public async Task<User> GetUserByPasswordResetTokenAsync(string passwordResetToken)
        {
            //Gör en try catch eller returna ett fel om användaren inte hittas
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == passwordResetToken);
		}

		public async Task<User>GetUserByIdAsync(int ownerId)
		{
			return await _context.Users
                .Include(c => c.NotificationSetting)
                .FirstOrDefaultAsync(u => u.Id == ownerId);
		}

		public async Task RemoveUserAsync(int userId)
		{
			await _context.Users
				.Where(u => u.Id == userId)
				.ExecuteDeleteAsync();

			await _context.CalendarAccesses
				.Where(ca => ca.UserId == userId)
				.ExecuteDeleteAsync();

			await _context.Calendars
				.Where(c => c.OwnerId == userId)
				.ExecuteDeleteAsync();

			await _context.SaveChangesAsync();
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
		public async Task SetLastLoggedInAsync(User user)
		{
			user.LastLoggedInUtc = DateTime.UtcNow;
            _context.Entry(user).Property(u => u.LastLoggedInUtc).IsModified = true;
            await _context.SaveChangesAsync();
		}

		public async Task SendPasswordResetEmailAsync(string email)
		{
			var user = await GetUserByEmailForPasswordAsync(email);
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
