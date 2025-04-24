using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.OwnedCalendars)
                .Include(u => u.CalendarAccesses)
                .ToListAsync();
        }
    }
}
