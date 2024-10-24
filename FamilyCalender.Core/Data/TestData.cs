using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;

namespace FamilyCalender.Core.Data
{
    public static class TestData
    {
        public static List<Member> GetTestMembers()
        {
            return
            [
                new Member { Name = "Robin" },
                new Member { Name = "Jenny" },
                new Member { Name = "Ida" },
                new Member { Name = "Maja" }
            ];
        }
        public static List<Calendar> GetTestCalendars(List<Member> members)
        {
            var calendar = new Calendar { Name = "Familjekalender" };

            var memberCalendars = new List<MemberCalendar>();
            foreach (var member in members)
            {
                memberCalendars.Add(new MemberCalendar
                {
                    Member = member,
                    Calendar = calendar
                });
            }

            calendar.MemberCalendars = memberCalendars;

            return [calendar];
        }

        public static List<Event> GetTestEvents(List<Member> members, Calendar calendar)
        {
            var events = new List<Event>
            {
                new() { Title = "Bokad aktivitet", EventDates = new List<DateTime> { new(2024, 10, 5) }, Calendar = calendar },
                new() { Title = "Läxor", EventDates = new List<DateTime> { new DateTime(2024, 10, 15) }, Calendar = calendar },
                new() { Title = "Skolprojekt", EventDates = new List<DateTime> { new DateTime(2024, 11, 10) }, Calendar = calendar },
                new() { Title = "Dammsuga", EventDates = new List<DateTime> { new DateTime(2024, 11, 11) }, Calendar = calendar }
            };

            for (int i = 0; i < events.Count; i++)
            {
                events[i].MemberEvents =
                [
                    new MemberEvent
                    {
                        Member = members[i % members.Count],
                        Event = events[i]                    }
                ];
            }

            return events;
        }
    }
}
