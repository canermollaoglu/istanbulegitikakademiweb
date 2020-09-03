using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days
{
    public class GroupLessonDayUpdatePostVm
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public DateTime DateOfLesson { get; set; }
        public bool HasAttendanceRecord { get; set; }
        public string EducatorId { get; set; }
        public Guid? ClassroomId { get; set; }
        public decimal? EducatorSalary { get; set; }


    }
}
