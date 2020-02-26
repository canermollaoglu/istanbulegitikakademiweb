using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days
{
    public class GroupLessonDayManagementVm
    {
        public EducationGroup Group { get; set; }
        public List<_GroupLessonDayEducator> Educators { get; set; }
        public List<_GroupLessonDayClassroom> Classrooms { get; set; }
    }

    public class _GroupLessonDayEducator
    {
        public string EducatorId { get; set; }
        public string FullName { get; set; }
    }
    public class _GroupLessonDayClassroom
    {
        public Guid ClassroomId { get; set; }
        public string Name { get; set; }
    }
}
