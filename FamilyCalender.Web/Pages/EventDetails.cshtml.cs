using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class EventDetailsModel(IEventService eventService, IMemberService memberService) : PageModel
    {
		private readonly IEventService _eventService = eventService;
		private readonly IMemberService _memberService = memberService;

		public Event? EventDetails { get; private set; }
        public Member? Member { get; set; }
		public DateTime? Day { get; set; }
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

			var selectedDays = EventDetails.EventDates
				.Where(ed => ed.Date >= DateTime.Today)
				.Select(ed => ed.Date.DayOfWeek)
				.Distinct()
				.ToList();

			SelectedDays = selectedDays;

			return Page();
        }

		public async Task<IActionResult> OnPostUpdateEventAsync(List<int> selectedMemberIds)
		{
			if(!ModelState.IsValid)
			{
				return Page();
			}

			var eventToUpdate = await _eventService.GetEventByIdAsync(EventId);
			if (eventToUpdate == null)
			{
				return NotFound();
			}

			eventToUpdate.Title = NewTitle;

			if (UpdateInterval)
			{
				var daysOfWeek = SelectedDays;
				var newStartDate = StartDate; 
				var newEndDate = EndDate;

				
				eventToUpdate.EventDates.Clear();

				for (var date = newStartDate; date <= newEndDate; date = date.AddDays(1))
				{
					if (daysOfWeek.Contains(date.DayOfWeek))
					{
						eventToUpdate.EventDates.Add(new EventDate
						{
							Date = date,
							EventId = eventToUpdate.Id
						});
					}
				}
			}
			else
			{
				eventToUpdate.EventDates[0].Date = NewDate;
			}

			var selectedMembers = selectedMemberIds.Select(id => new MemberEvent
			{
				MemberId = id,
				EventId = eventToUpdate.Id
			}).ToList();

			eventToUpdate.MemberEvents = selectedMembers;

			await _eventService.UpdateEventAsync(eventToUpdate);

			return RedirectToPage("./EventDetails", new { eventId = eventToUpdate.Id });
		}
	}
}
