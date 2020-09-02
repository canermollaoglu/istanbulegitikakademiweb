using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days
{
    public class GroupLessonDayGetVm
    {
        public Guid Id { get; set; }
        public DateTime DateOfLesson { get; set; }
        public decimal EducatorSalary { get; set; }
        public string  EducatorFullName { get; set; }
        public string ClassRoomName { get; set; }
    }
}
