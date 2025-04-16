using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class CalendarAccessService : ICalendarAccessService
    {
        private readonly ICalendarAccessRepository _calendarAccessRepository;

        public CalendarAccessService(ICalendarAccessRepository calendarAccessRepository)
        {
            _calendarAccessRepository = calendarAccessRepository;
        }
        public async Task CreateCalendarAccessAsync(CalendarAccess access)
        {
            await _calendarAccessRepository.AddAsync(access);
		}
        public async Task RemoveUserFromCalendarAccessAsync(int currentUserId, int calendarId)
        {
            await _calendarAccessRepository.RemoveAsync(currentUserId, calendarId);
        }
	}
}
