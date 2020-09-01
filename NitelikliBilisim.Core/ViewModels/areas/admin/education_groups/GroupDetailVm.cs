﻿using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.groups;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class GroupDetailVm
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime? StartDate { get; set; }
        public byte Quota { get; set; } = 0;
        public _Education Education { get; set; }
        public EducationHost Host { get; set; }
        public Classroom ClassRoom { get; set; } = new Classroom();
        public List<GroupLessonDay> LessonDays { get; set; }
        public List<GroupExpense> Expenses { get; set; }
        public List<Customer> Students { get; set; } = new List<Customer>();
        public string EducatorName { get; set; }
        public List<GroupExpenseType> GroupExpenseTypes { get; set; }
    }
    
}
