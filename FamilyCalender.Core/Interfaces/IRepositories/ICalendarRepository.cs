using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar = FamilyCalender.Core.Models.Entities.Calendar;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface ICalendarRepository
    {
        Task<Calendar> GetByIdAsync(int calendarId);
        Task<List<int>> GetAllIdsByUserAsync(int userId);
        Task<Calendar> AddAsync(Calendar calendar);
        Task RemoveAsync(int calendarId);
        Task <CalendarDto> GetCalendarDtoAsync(int calendarId);
        Task<List<CalendarDto>> GetCalendarDtosAsync(int userId);
        Task<Calendar?> GetCalendarWithAllRelationsAsync(int calendarId);
		Task<Calendar?> GetByIdWithDetailsAsync(int calendarId);
        Task UpdateAsync(Calendar calendar);
        Task DeleteAsync(Calendar calendar);
		Task<Calendar> UpdateCreateAsync(Calendar calendar);




	}
}
