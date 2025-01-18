using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class EventDetailsModel(UserManager<User> userManager, IEventService eventService, IMemberService memberService) : PageModel
    {
		private readonly UserManager<User> _userManager = userManager;
		private readonly IEventService _eventService = eventService;
		private readonly IMemberService _memberService = memberService;

		public Event? EventDetails { get; private set; }
        public Member? Member { get; set; }


        public async Task<IActionResult> OnGetAsync(int eventId, int memberId)
		{
			EventDetails = await _eventService.GetEventByIdAsync(eventId);

			Member = await _memberService.GetMemberByIdAsync(memberId);

			if (EventDetails == null)
			{
				return NotFound();
			}

			return Page();
        }
    }
}
