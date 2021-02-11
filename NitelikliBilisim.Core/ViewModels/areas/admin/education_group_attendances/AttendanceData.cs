using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_group_attendances
{
    public class AttendanceData
    {
        public Guid GroupId { get; set; }
        public DateTime Date { get; set; }
        public List<StudentRecord> StudentRecords { get; set; }
    }
    public class StudentRecord
    {
        public string CustomerId { get; set; }
        public bool IsAttended { get; set; }
        public string Reason { get; set; }
    }
}
