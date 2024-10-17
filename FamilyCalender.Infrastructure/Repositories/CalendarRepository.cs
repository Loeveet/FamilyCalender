﻿using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly ApplicationDbContext _context;

        public CalendarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Calendar> AddAsync(Calendar calendar)
        {
            //calendar.CreatedAt = DateTime.UtcNow;
            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();
            return calendar;
        }

        public async Task<List<Calendar>> GetAllByUserAsync(string userId)
        {
            var calendarAccesses = await _context.CalendarAccesses
                .Where(ca => ca.UserId == userId)
                .Include(ca => ca.Calendar) 
                .ToListAsync();

            var calendars = calendarAccesses
                .Select(ca => ca.Calendar)
                .Distinct()
                .ToList();

            return calendars;

        }

        public async Task<Calendar> GetByIdAsync(int calendarId)
        {
            return await _context.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
        }

        public async Task RemoveAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

        public async Task<Calendar> UpdateAsync(Calendar calendar)
        {
            var existingCalendar = await _context.Calendars
                .Include(c => c.MemberCalendars)
                .FirstOrDefaultAsync(c => c.Id == calendar.Id);

            if (existingCalendar != null)
            {
                _context.Calendars.Update(calendar);
                await _context.SaveChangesAsync();
                return calendar;
            }
            return null;
        }
    }
}
