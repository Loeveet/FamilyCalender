﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
    public class EventMemberDate
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public int MemberId { get; set; }
        public Member? Member { get; set; }

        public DateTime Date { get; set; }
    }
}
