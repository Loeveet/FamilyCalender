using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
	public class EventManagementService
	{
		private readonly IEventService _eventService;
		private readonly IMemberService _memberService;

		public EventManagementService(IEventService eventService, IMemberService memberService)
		{
			_eventService = eventService;
			_memberService = memberService;
		}

		public async Task<Event?> GetEventDetailsAsync(int eventId)
		{
			return await _eventService.GetEventByIdAsync(eventId);
		}

		public async Task<Member?> GetMemberAsync(int memberId)
		{
			return await _memberService.GetMemberByIdAsync(memberId);
		}

		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
		{
			return await _memberService.GetMembersForCalendarAsync(calendarId);
		}

		public async Task UpdateEventAsync(Event eventToUpdate, List<int> selectedMemberIds, string? editOption, DateTime startDate, DateTime endDate, DateTime newDate, List<string> selectedDays)
		{
			eventToUpdate.EventMemberDates.Clear();

			if (editOption == "all")
			{
				var selectedDaysLower = selectedDays
					.Select(d => d.ToLower())
					.ToHashSet();
				for (var date = startDate; date <= endDate; date = date.AddDays(1))
				{
					var dayName = new CultureInfo("sv-SE").DateTimeFormat.GetDayName(date.DayOfWeek).ToLower();

					if (selectedDaysLower.Contains(dayName))
					{
						foreach (var memberId in selectedMemberIds)
						{
							eventToUpdate.EventMemberDates.Add(new EventMemberDate
							{
								Date = date,
								MemberId = memberId,
								EventId = eventToUpdate.Id
							});
						}
					}
				}
			}
			else
			{
				foreach (var memberId in selectedMemberIds)
				{
					eventToUpdate.EventMemberDates.Add(new EventMemberDate
					{
						Date = newDate,
						MemberId = memberId,
						EventId = eventToUpdate.Id
					});
				}
			}

			await _eventService.UpdateEventAsync(eventToUpdate);
		}

		public async Task DeleteEventAsync(int eventId, int memberId, DateTime day, List<int> selectedMemberIds, string? deleteOption)
		{
			if (selectedMemberIds.Count == 0)
			{
				if (deleteOption == "single")
				{
					await _eventService.DeleteEventMemberDateAsync(eventId, memberId, day);
				}
				else
				{
					await _eventService.DeleteEventAsync(eventId);
				}
			}
			else if (selectedMemberIds.Count > 0)
			{
				var members = await _memberService.GetMembersByIdAsync(selectedMemberIds);

				if (deleteOption == "all")
				{
					foreach (var member in members)
					{
						await _eventService.DeleteAllEventMemberDatesAsync(eventId, member.Id);
					}
				}
				else
				{
					foreach (var member in members)
					{
						if (member.EventMemberDates.Any(emd => emd.EventId == eventId && emd.Date == day))
						{
							await _eventService.DeleteEventMemberDateAsync(eventId, member.Id, day);
						}
					}
				}
			}
		}
	}
}
