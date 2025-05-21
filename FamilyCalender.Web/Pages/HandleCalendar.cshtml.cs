using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static FamilyCalender.Web.ViewModels.CalendarOverViewViewModel;
using System.Globalization;

namespace FamilyCalender.Web.Pages
{
	public class HandleCalendarModel(CalendarManagementService calendarManagementService, 
		IAuthService authService, 
		IMemberService memberService,
		ICalendarAccessService calendarAccessService) : BasePageModel(authService)
	{
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;
		private readonly IMemberService _memberService = memberService;
		private readonly ICalendarAccessService _calendarAccessService = calendarAccessService;

		[BindProperty]
		public Core.Models.Entities.Calendar? Calendar { get; set; }

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
		[BindProperty]
		public bool IsOwner { get; set; }
		[BindProperty]
		public User? CurrentUser { get; set; }
		[BindProperty]
		public string ShareLink { get; set; }
		[BindProperty]
		public List<User?> CalendarUsers { get; set; } = [];
		[BindProperty]
		public CalendarView SelectedView { get; set; }
		public bool DontScrollToToday { get; set; }


		public async Task<IActionResult> OnGetAsync(int id)
		{
			var user = await GetCurrentUserAsync();
			if (user == null)
            {
                return RedirectToPage("/Login");
            }

			var calendar = await _calendarManagementService.GetCalendarWithDetailsAsync(id);
            if (calendar == null)
			{
                return RedirectToPage("/CalendarOverview");
            }

			var calendarAccess = await _calendarAccessService.GetCalendarAccessAsync(user.Id, id);

			if (calendarAccess?.Settings != null)
			{
				SelectedView = calendarAccess.Settings.PreferWeekView
								? CalendarView.Week
								: CalendarView.Month;
				DontScrollToToday = calendarAccess.Settings.DontScrollToToday;
			}
			else
			{
				SelectedView = CalendarView.Month;
			}

			if (calendar?.InviteId != null)
			{
				ShareLink = $"{Request.Scheme}://{Request.Host}/Invite/{calendar.InviteId}";
			}
			IsOwner = calendar.OwnerId == user.Id || calendar.Accesses.Any(a => a.UserId == user.Id && a.IsOwner);

			if (IsOwner)
			{
				CalendarUsers = calendar.Accesses
					.Select(a => a.User)
					.Where(u => u.Id != calendar.OwnerId)
					.Distinct()
					.ToList();
			}
			else
			{
				var ownerUser = await _authService.GetUserByIdAsync(calendar.OwnerId);
				if (ownerUser != null)
				{
					CalendarUsers = [ownerUser];
				}
			}

			CurrentUser = user;
            Calendar = calendar;
			NewCalendarName = calendar.Name;
			Members = await _memberService.GetMembersForCalendarAsync(id);


			return Page();
		}
		public async Task<IActionResult> OnPostChangeViewAsync(string? view)
		{
			var user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToPage("/Login");
			}

			if (!string.IsNullOrEmpty(view))
			{
				var calendarAccess = await _calendarAccessService.GetCalendarAccessAsync(user.Id, Calendar.Id);
				if (calendarAccess != null)
				{
					if (calendarAccess.Settings == null)
					{
						calendarAccess.Settings = new UserSettings();
					}

					calendarAccess.Settings.PreferWeekView = view.Equals("Week", StringComparison.OrdinalIgnoreCase);

					await _calendarAccessService.UpdateCalendarAccessSettingsAsync(calendarAccess.Settings);

				}
			}
			return RedirectToPage(new { Calendar.Id });
		}
		public async Task<IActionResult> OnPostToggleScrollAsync()
		{
			var user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToPage("/Login");
			}

			var calendarAccess = await _calendarAccessService.GetCalendarAccessAsync(user.Id, Calendar.Id);
			if (calendarAccess != null)
			{
				if (calendarAccess.Settings == null)
				{
					calendarAccess.Settings = new UserSettings();
				}

				// Toggle v�rdet
				calendarAccess.Settings.DontScrollToToday = !calendarAccess.Settings.DontScrollToToday;

				await _calendarAccessService.UpdateCalendarAccessSettingsAsync(calendarAccess.Settings);
			}

			return RedirectToPage(new { Calendar.Id });
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
			return RedirectToPage("/CalendarOverview");
		}

		public async Task<IActionResult> OnPostDeleteMemberAsync()
		{
			await _memberService.DeleteMemberAsync(MemberId);
			return RedirectToPage(new { Calendar.Id });
		}

		public async Task<IActionResult> OnPostRenameMemberAsync()
		{
			if (string.IsNullOrWhiteSpace(NewMemberName))
			{
				return RedirectToPage(new { Calendar.Id });
			}
			await _memberService.UpdateMemberNameAsync(MemberIdToEdit, NewMemberName);
			return RedirectToPage(new { Calendar.Id });
		}

		public async Task<IActionResult> OnPostAddMemberAsync()
		{
			if (string.IsNullOrWhiteSpace(NewMemberName))
			{
				return RedirectToPage(new { Calendar.Id });
			}
			var user = await GetCurrentUserAsync();
			await _calendarManagementService.AddMemberAsync(NewMemberName, Calendar.Id, user);
			return RedirectToPage(new { id = Calendar.Id });
		}

		public async Task<IActionResult> OnPostRemoveAccessAsync(int calendarId, int userId)
		{
			var isSelf = userId == CurrentUser.Id;
			var isOwner = await _calendarManagementService.IsCalendarOwnerAsync(calendarId, CurrentUser.Id);

			await _calendarManagementService.RemoveUserFromCalendarAsync(userId, calendarId);
			await SetNewInviteGuid(calendarId);

			if (isSelf && !isOwner)
			{
				return RedirectToPage("/CalendarOverview");
			}
			else if (isOwner || isSelf)
			{
				return RedirectToPage(new { id = calendarId });
			}

			return RedirectToPage("/CalendarOverview");
		}

		private async Task SetNewInviteGuid(int calendarId)
		{
			var cal = await _calendarManagementService.GetCalendarByCalendarIdAsync(calendarId);
			cal.InviteId = Guid.NewGuid();
			await _calendarManagementService.UpdateCalenderInviteIdAsync(cal);
		}


	}

}