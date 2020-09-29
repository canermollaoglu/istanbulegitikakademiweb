using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days
{
    public class GroupLessonDayUpdateGetVm
    {
        public Guid Id { get; set; }
        public string EducatorId { get; set; }
        public decimal? EducatorSalary { get; set; }
        public Guid? ClassRoomId { get; set; }
        public bool HasAttendanceRecord { get; set; }
        public DateTime DateOfLesson { get; set; }
        public Dictionary<string,string> Educators { get; set; }
        public Dictionary<Guid,string> ClassRooms { get; set; }
        public string GroupName { get; set; }
        public Guid GroupId { get; set; }
    }
}
