using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("GroupLessonDays")]
    public class GroupLessonDay : BaseEntity<Guid>
    {
        public GroupLessonDay()
        {
            Id = Guid.NewGuid();
        }

        [ForeignKey("Group")]
        public Guid GroupId { get; set; }
        public DateTime DateOfLesson { get; set; }
        public bool HasAttendanceRecord { get; set; }
        public bool IsImmuneToAutoChange { get; set; }
        [MaxLength(128)]
        public string EducatorId { get; set; }
        public Guid? ClassroomId { get; set; }

        public virtual EducationGroup Group { get; set; }
    }
}
