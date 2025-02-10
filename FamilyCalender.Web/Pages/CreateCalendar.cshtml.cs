using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    [Authorize]
    public class CreateCalendarModel(ICalendarService calendarService, IMemberService memberService, UserManager<User> userManager) : PageModel
    {
        private readonly ICalendarService _calendarService = calendarService;
        private readonly IMemberService _memberService = memberService;
        private readonly UserManager<User> _userManager = userManager;

        [BindProperty]
        public Calendar Calendar { get; set; } = new Calendar();

        [BindProperty]
        public List<Member> Members { get; set; } = [];

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
            try
            {
                var createdCalendar = await _calendarService.CreateCalendarAsync(Calendar, user);
                if (Members.Count > 0)
                {
                    var memberCalendars = new List<MemberCalendar>();

                    foreach (var member in Members)
                    {
                        if (!string.IsNullOrEmpty(member.Name))
                        {
                            var createdMember = await _memberService.CreateMemberAsync(member, user);

                            memberCalendars.Add(new MemberCalendar
                            {
                                Member = createdMember,
                                MemberId = createdMember.Id,
                                Calendar = createdCalendar,
                                CalendarId = createdCalendar.Id
                            });
                        }
                    }
                    createdCalendar.MemberCalendars = memberCalendars;
                    await _calendarService.UpdateCalendarAsync(createdCalendar);
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
