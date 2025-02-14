﻿using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface ICalendarRepository
    {
        Task<Calendar> GetByIdAsync(int calendarId);
        Task<List<int>> GetAllIdsByUserAsync(string userId);
        Task<Calendar> AddAsync(Calendar calendar);
        Task<Calendar> UpdateAsync(Calendar calendar);
        Task RemoveAsync(int calendarId);


	}
}
