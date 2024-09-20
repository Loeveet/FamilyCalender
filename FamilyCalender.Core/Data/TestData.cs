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

        public static List<Event> GetTestEvents(List<Member> members)
        {
            return new List<Event>
            {
                new Event { Title = "Bokad aktivitet", Start = new DateTime(2024, 9, 5), Member = members[0] },
                new Event { Title = "Läxor", Start = new DateTime(2024, 9, 15), Member = members[1] },
                new Event { Title = "Skolprojekt", Start = new DateTime(2024, 10, 10), Member = members[2] },
                new Event { Title = "Dammsuga", Start = new DateTime(2024, 10, 11), Member = members[3] }
            };
        }
    }
}
