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
	public class HandleCalendarModel(CalendarManagementService calendarManagementService, IAuthService authService) : BasePageModel(authService)
	{
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;

		[BindProperty]
		public Calendar? Calendar { get; set; }

		[BindProperty]
		public string NewCalendarName { get; set; } = string.Empty;

		public async Task<IActionResult> OnGetAsync(int id)
		{
			var user = await GetCurrentUserAsync();
			if (user == null) return RedirectToPage("/Login");

			var calendar = await _calendarManagementService.GetCalendarWithDetailsAsync(id);
			if (calendar == null || calendar.OwnerId != user.Id)
				return RedirectToPage("/Index");

			Calendar = calendar;
			NewCalendarName = calendar.Name;

			return Page();
		}

		public async Task<IActionResult> OnPostUpdateNameAsync()
		{
			if (Calendar == null)
			{
				return NotFound();
			}

			await _calendarManagementService.UpdateCalendarNameAsync(Calendar.Id, NewCalendarName);
			return RedirectToPage(new { Calendar.Id });
		}

		public async Task<IActionResult> OnPostDeleteCalendarAsync()
		{
			if (Calendar == null)
			{
				return NotFound();
			}

			await _calendarManagementService.DeleteCalendarAsync(Calendar.Id);
			return RedirectToPage("/Index");
		}
	}

}