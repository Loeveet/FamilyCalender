using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Packaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyCalender.Web.Pages
{
	public class EventDetailsModel(IEventService eventService, IMemberService memberService) : PageModel
	{
		private readonly IEventService _eventService = eventService;
		private readonly IMemberService _memberService = memberService;

		public Event? EventDetails { get; private set; }
		[BindProperty]
		public Member? Member { get; set; }
		[BindProperty]
		public int MemberId { get; set; }
		[BindProperty]
		public DateTime Day { get; set; }
		public List<Member> Members { get; set; } = [];
		[BindProperty]
		public string? NewTitle { get; set; }
		[BindProperty]
		public List<DayOfWeek> SelectedDays { get; set; } = [];
		[BindProperty]
		public bool UpdateInterval { get; set; }
		[BindProperty]
		public DateTime StartDate { get; set; }
		[BindProperty]
		public DateTime EndDate { get; set; }
		[BindProperty]
		public DateTime NewDate { get; set; }
		[BindProperty]
		public int EventId { get; set; }
		[BindProperty]
		public int CalendarId { get; set; }
		[BindProperty]
		public int Year { get; set; }
		[BindProperty]
		public int Month { get; set; }



		public async Task<IActionResult> OnGetAsync(int eventId, int memberId, DateTime day)
		{
			EventDetails = await _eventService.GetEventByIdAsync(eventId);

			if (EventDetails == null)
			{
				return NotFound();
			}
			var calendarId = EventDetails.CalendarId;

			Member = await _memberService.GetMemberByIdAsync(memberId);

			Day = day;
			Members = await _memberService.GetMembersForCalendarAsync(calendarId);

			var selectedDays = EventDetails.EventMemberDates
				.Where(ed => ed.Date >= DateTime.Today)
				.Select(ed => ed.Date.DayOfWeek)
				.Distinct()
				.ToList();

			SelectedDays = selectedDays;

			return Page();
		}

		public async Task<IActionResult> OnPostUpdateEventAsync(List<int> selectedMemberIds, string? editOption)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			var eventToUpdate = await _eventService.GetEventByIdAsync(EventId);
			if (eventToUpdate == null)
			{
				return NotFound();
			}

			eventToUpdate.Title = NewTitle;
			var events = eventToUpdate.EventMemberDates.ToList();
			eventToUpdate.EventMemberDates.Clear();
			

			if (editOption == "all")
			{

				var daysOfWeek = SelectedDays;
				var newStartDate = StartDate;
				var newEndDate = EndDate;

				for (var date = newStartDate; date <= newEndDate; date = date.AddDays(1))
				{
					if (daysOfWeek.Contains(date.DayOfWeek))
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
						Date = NewDate,
						MemberId = memberId,
						EventId = eventToUpdate.Id
					});
				}
			}
			await _eventService.UpdateEventAsync(eventToUpdate);
			if (!eventToUpdate.EventMemberDates.Any(e => e.Date == Day) && editOption == "all")
			{
				return RedirectToPage("./Index", new { year = Day.Year, month = Day.Month, calendarId = CalendarId });
			}
			return RedirectToPage("./EventDetails", new { eventId = eventToUpdate.Id, memberId = Member.Id, day = NewDate });
		}
		public async Task<IActionResult> OnPostDeleteEventAsync(List<int> selectedMemberIds, string? deleteOption)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (selectedMemberIds.Count == 0)
			{
				if (deleteOption == "single")
				{
					await _eventService.DeleteEventMemberDateAsync(EventId, Member.Id, Day);
				}

				else
				{
					await _eventService.DeleteEventAsync(EventId);
				}
			}
			else if (selectedMemberIds.Count > 0)
			{
				var members = await _memberService.GetMembersByIdAsync(selectedMemberIds);

				if (deleteOption == "all")
				{
					foreach (var member in members)
					{
						await _eventService.DeleteAllEventMemberDatesAsync(EventId, member.Id);
					}
				}
				else
				{
					foreach (var member in members)
					{
						if(member.EventMemberDates.Any(emd => emd.EventId == EventId && emd.Date == Day))
						{
							await _eventService.DeleteEventMemberDateAsync(EventId, member.Id, Day);
						}
					}
				}
			}

			return RedirectToPage("./Index", new { year = Day.Year, month = Day.Month, calendarId = CalendarId });


		}

	}
}
