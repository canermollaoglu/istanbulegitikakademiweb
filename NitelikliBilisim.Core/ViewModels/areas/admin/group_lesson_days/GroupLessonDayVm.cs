using System;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days
{
    public class GroupLessonDayVm
    {
        public Guid Id { get; set; }
        public DateTime DateOfLesson { get; set; }
        public string DateOfLessonText { get; set; }
        public bool HasAttendanceRecord { get; set; }
        public string Classroom { get; set; }
        public string EducatorName { get; set; }
        public int HoursPerDay { get; set; }
    }
}
