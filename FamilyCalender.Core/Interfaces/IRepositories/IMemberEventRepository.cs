using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
	public interface IMemberEventRepository
	{
		Task AddAsync(MemberEvent memberEvent);
	}
}
