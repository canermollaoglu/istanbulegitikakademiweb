using NitelikliBilisim.Core.Entities;
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
        public string StartDate { get; set; }
        public byte Quota { get; set; } = 0;
        public _Education Education { get; set; }
        public EducationHost Host { get; set; }
        public string EducatorName { get; set; }
        public List<GroupExpenseType> GroupExpenseTypes { get; set; }
        public string ClassRoomName { get; set; }
        public int AssignedStudentsCount { get; set; }
        public Dictionary<string, string> SelectEducators { get; set; }
        public int MinimumStudentCount { get; set; }
        public string EducationDays { get; set; }
        public string EducationHoursPerDay { get; set; }
        public string EndDate { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? NewPrice { get; set; }
        public int ExpectedProfitRate { get; set; }
        public decimal ExpectedSellingPrice { get; set; }
    }
    
}
