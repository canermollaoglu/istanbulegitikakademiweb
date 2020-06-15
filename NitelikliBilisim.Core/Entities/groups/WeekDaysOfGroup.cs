using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("WeekDaysOfGroups")]
    public class WeekDaysOfGroup : BaseEntity<Guid>
    {
        public WeekDaysOfGroup()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(128)]
        public string DaysJson { get; set; }

        [ForeignKey("Group")]
        public Guid GroupId { get; set; }
        public virtual EducationGroup Group { get; set; }
    }
}
