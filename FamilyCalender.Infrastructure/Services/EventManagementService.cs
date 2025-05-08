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
	public class EventManagementService(IEventService eventService, IMemberService memberService)
	{
		private readonly IEventService _eventService = eventService;
		private readonly IMemberService _memberService = memberService;

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

		public async Task UpdateEventAsync(Event eventToUpdate)
		{
			ArgumentNullException.ThrowIfNull(eventToUpdate);
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
