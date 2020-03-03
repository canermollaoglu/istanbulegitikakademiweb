using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.educator_area.group
{
    public class GroupDetailsVm
    {
        public _Group Group { get; set; }
        public List<_EducationDay> Days { get; set; }
        public List<_GroupStudent> GroupStudents { get; set; }
    }

    public class _EducationDay
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DateText { get; set; }
        public bool HasAttendanceRecord { get; set; }
    }
    public class _GroupStudent
    {
        public string FullName { get; set; }
    }
}
