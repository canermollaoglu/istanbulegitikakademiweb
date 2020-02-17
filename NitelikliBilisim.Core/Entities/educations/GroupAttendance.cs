using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("GroupAttendances")]
    public class GroupAttendance : BaseEntity<Guid>
    {
        [ForeignKey(nameof(Id))]
        public virtual EducationGroup Group { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(450)]
        public string CustomerId { get; set; }
    }
}
