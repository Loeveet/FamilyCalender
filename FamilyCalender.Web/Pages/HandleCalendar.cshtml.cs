using FamilyCalender.Core.Interfaces;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json.Linq;
using System;

namespace FamilyCalender.Web.Pages
{
	public class HandleCalendarModel(CalendarManagementService calendarManagementService, IAuthService authService, IMemberService memberService) : BasePageModel(authService)
	{
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;
		private readonly IMemberService _memberService = memberService;

		[BindProperty]
		public Calendar? Calendar { get; set; }

		[BindProperty]
		public string NewCalendarName { get; set; } = string.Empty;

		[BindProperty]
		public List<Member> Members { get; set; } = [];

		[BindProperty]
		public string NewMemberName { get; set; } = string.Empty;

		[BindProperty]
		public int MemberIdToEdit { get; set; }
		[BindProperty]
		public int MemberId { get; set; }

		public async Task<IActionResult> OnGetAsync(int id)
		{
			var user = await GetCurrentUserAsync();
			if (user == null) return RedirectToPage("/Login");

			var calendar = await _calendarManagementService.GetCalendarWithDetailsAsync(id);
			if (calendar == null || calendar.OwnerId != user.Id)
				return RedirectToPage("/Index");

			Calendar = calendar;
			NewCalendarName = calendar.Name;
			Members = await _memberService.GetMembersForCalendarAsync(id);


			return Page();
		}

		public async Task<IActionResult> OnPostUpdateNameAsync()
		{
			if (Calendar == null)
			{
				throw new ArgumentNullException(nameof(Calendar));
			}

			await _calendarManagementService.UpdateCalendarNameAsync(Calendar.Id, NewCalendarName);
			return RedirectToPage(new { Calendar.Id });
		}

		public async Task<IActionResult> OnPostDeleteCalendarAsync()
		{
			if (Calendar == null)
			{
				throw new ArgumentNullException(nameof(Calendar));
			}

			await _calendarManagementService.DeleteCalendarAsync(Calendar.Id);
			return RedirectToPage("/Index");
		}

		public async Task<IActionResult> OnPostDeleteMemberAsync()
		{
			await _memberService.DeleteMemberAsync(MemberId);
			return RedirectToPage(new { Calendar.Id });
		}

		public async Task<IActionResult> OnPostRenameMemberAsync()
		{
			await _memberService.UpdateMemberNameAsync(MemberIdToEdit, NewMemberName);
			return RedirectToPage(new { Calendar.Id });
		}

		public async Task<IActionResult> OnPostAddMemberAsync()
		{
			var user = await GetCurrentUserAsync();
			await _calendarManagementService.AddMemberAsync(NewMemberName, Calendar.Id, user);
			return RedirectToPage(new { id = Calendar.Id });
		}

	}

}