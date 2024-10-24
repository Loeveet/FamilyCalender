using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
	public class MemberEventService : IMemberEventService
	{
		private readonly IMemberEventRepository _memberEventRepository;
		private readonly IMemberService _memberService;
		public MemberEventService(IMemberEventRepository memberEventRepository, IMemberService memberService)
        {
			_memberEventRepository = memberEventRepository;
			_memberService = memberService;
		}
        public async Task CreateMemberEventAsync(int memberId, Event e)
		{
			var memberEvent = new MemberEvent
			{
				MemberId = memberId,
				EventId = e.Id,
				Member = await _memberService.GetMemberByIdAsync(memberId), // Hämta medlem från databas
				Event = e,
			};
			await _memberEventRepository.AddAsync(memberEvent);
		}
	}
}
