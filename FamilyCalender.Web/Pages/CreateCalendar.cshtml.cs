using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    [Authorize]
    public class CreateCalendarModel : PageModel
    {
        private readonly ICalendarService _calendarService;
        private readonly IMemberService _memberService;

        [BindProperty]
        public Calendar Calendar { get; set; }

        public CreateCalendarModel(ICalendarService calendarService, IMemberService memberService)
        {
            _calendarService = calendarService;
            _memberService = memberService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Calendar.Members != null && Calendar.Members.Count > 0)
            {
                foreach (var member in Calendar.Members)
                {
                    if (!string.IsNullOrEmpty(member.Name)) 
                    {
                        await _memberService.CreateMemberAsync(member); 
                    }
                }
            }

            try
            {
                await _calendarService.CreateCalendarAsync(Calendar);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page(); 
            }

            return RedirectToPage("/Index");
        }

    }

}
