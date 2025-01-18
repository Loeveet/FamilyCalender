using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class EventDetailsModel(UserManager<User> userManager, IEventService eventService) : PageModel
    {
		private readonly UserManager<User> _userManager = userManager;
		private readonly IEventService _eventService = eventService;

		public Event? EventDetails { get; private set; }

		public async Task<IActionResult> OnGetAsync(int id)
		{
			EventDetails = await _eventService.GetEventByIdAsync(id);

			if (EventDetails == null)
			{
				return NotFound();
			}

			return Page();
        }
    }
}
