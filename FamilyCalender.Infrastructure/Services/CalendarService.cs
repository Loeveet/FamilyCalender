using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
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
        private readonly ICalendarAccessService _calendarAccessService;

        public CalendarService(ICalendarRepository calendarRepository, ICalendarAccessService calendarAccessService)
        {
            _calendarRepository = calendarRepository;
            _calendarAccessService = calendarAccessService;
        }

        public async Task<Calendar> CreateCalendarAsync(Calendar calendar, User user)
        {
            if (string.IsNullOrWhiteSpace(calendar.Name))
            {
                throw new ArgumentException("Calendar name cannot be empty.");
            }

            calendar.OwnerId = user.Id;
            calendar.Owner = user;

            try
            {
                var newCalendar = await _calendarRepository.AddAsync(calendar);

                var access = new CalendarAccess()
                {
                    Calendar = newCalendar,
                    CalendarId = newCalendar.Id,
                    User = user,
                    UserId = user.Id,
                    IsOwner = true
                };

                await _calendarAccessService.CreateCalendarAccessAsync(access);

                return newCalendar;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Something went wrong while creating the calendar.", ex);
            }
        }

        public async Task<List<int>> GetCalendarIdsForUserAsync(int userId)
        {
            return await _calendarRepository.GetAllIdsByUserAsync(userId);
        }

        public Task<List<Event>> GetEventsForCalendarAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

		public async Task<Calendar> GetOneCalendarAsync(int calendarId)
		{
            return await _calendarRepository.GetByIdAsync(calendarId) ?? throw new FileNotFoundException();
		}

		public async Task<Calendar> UpdateCalendarAsync(Calendar calendar)
        {
            var updatedCalendar = await _calendarRepository.UpdateAsync(calendar);
			return updatedCalendar ?? throw new InvalidDataException(); //Kolla igenom exceptions.
		}
		public async Task<CalendarDto> GetCalendarDtoAsync(int calendarId)
        {
            return await _calendarRepository.GetCalendarDtoAsync(calendarId);
        }
        public async Task<List<CalendarDto>> GetCalendarDtosForUserAsync(int userId)
        {
            return await _calendarRepository.GetCalendarDtosAsync(userId);
        }


    }
}
