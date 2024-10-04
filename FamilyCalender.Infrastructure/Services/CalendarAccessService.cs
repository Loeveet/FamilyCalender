using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
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
    }
}
