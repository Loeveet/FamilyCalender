using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;

namespace FamilyCalender.Core.Data
{
    public static class TestData
    {
        public static List<Member> GetTestMembers()
        {
            return new List<Member>
            {
                new Member { Name = "Robin" },
                new Member { Name = "Jenny" },
                new Member { Name = "Ida" },
                new Member { Name = "Maja" }
            };
        }

        public static List<Event> GetTestEvents(List<Member> members, int year, int month)
        {
            return new List<Event>
            {
                new Event { Title = "Bokad aktivitet", Start = new DateTime(year, month, 5), Member = members[0] },
                new Event { Title = "Läxor", Start = new DateTime(year, month, 15), Member = members[1] },
                new Event { Title = "Skolprojekt", Start = new DateTime(year, month, 10), Member = members[2] },
                new Event { Title = "Dammsuga", Start = new DateTime(year, month, 11), Member = members[3] }
            };
        }
    }
}
