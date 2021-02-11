using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_group_attendances
{
    public class AttendanceVm
    {
        public Guid GroupId { get; set; }
        public DateTime Date { get; set; }
        public List<_Attendance> Attendances { get; set; } = new List<_Attendance>();
    }
    public class _Attendance
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsAttended { get; set; }
        public string Reason { get; set; }
    }
}
