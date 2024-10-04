using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    [Authorize]
    public class CreateCalendarModel : PageModel
    {
        private readonly ICalendarService _calendarService;
        private readonly IMemberService _memberService;
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public Calendar Calendar { get; set; }

        [BindProperty]
        public List<Member> Members { get; set; } = [];


        public CreateCalendarModel(ICalendarService calendarService, IMemberService memberService, UserManager<User> userManager)
        {
            _calendarService = calendarService;
            _memberService = memberService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = new User();
            try
            {
                user = await _userManager.GetUserAsync(User);
            }
            catch (Exception ex)
            {
                throw new Exception("No logged in user", ex);
            }
            var createdCalendar = new Calendar();
            try
            {
                createdCalendar = await _calendarService.CreateCalendarAsync(Calendar, user);
                if (Members.Count > 0)
                {
                    foreach (var member in Members)
                    {
                        if (!string.IsNullOrEmpty(member.Name))
                        {
                            var createdMember = await _memberService.CreateMemberAsync(member, user);
                            
                            Calendar.MemberCalendars.Add(new MemberCalendar
                            {
                                MemberId = createdMember.Id,
                                CalendarId = createdCalendar.Id
                            });
                        }
                    }
                }
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
