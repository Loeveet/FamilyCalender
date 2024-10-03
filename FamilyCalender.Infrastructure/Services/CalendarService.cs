using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly ICalendarRepository _calendarRepository;

        public CalendarService(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        public async Task<Calendar> CreateCalendarAsync(Calendar calendar)
        {
            if (string.IsNullOrWhiteSpace(calendar.Name))
            {
                throw new ArgumentException("Calendar name cannot be empty.");
            }
            if (calendar.OwnerId == 0)
            {
                throw new ArgumentException("Calendar must have an owner.");
            }
            try
            {
                return await _calendarRepository.AddAsync(calendar);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Something went wrong while creating the calendar.", ex);
            }
        }
    }
}
