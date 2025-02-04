using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
	//public class MemberEventRepository : IMemberEventRepository
	//{
	//	private readonly ApplicationDbContext _context;

	//	public MemberEventRepository(ApplicationDbContext context)
	//	{
	//		_context = context;
	//	}
	//	public async Task AddAsync(MemberEvent memberEvent)
	//	{
	//		try
	//		{
	//			await _context.AddAsync(memberEvent);
	//			await _context.SaveChangesAsync();
	//		}
	//		catch (Exception ex) 
	//		{
	//			throw new Exception("Something went wrong", ex);
	//		}
	//	}
	//}
}
