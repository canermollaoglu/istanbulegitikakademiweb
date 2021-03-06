using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.groups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationGroups")]
    public class EducationGroup : BaseEntity<Guid>
    {
        public EducationGroup()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(128)]
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
        [MaxLength(128)]
        public string EducatorId { get; set; }
        [ForeignKey("Education")]
        public Guid EducationId { get; set; }
        public virtual Education Education { get; set; }
        [ForeignKey("Host")]
        public Guid HostId { get; set; }
        public virtual EducationHost Host { get; set; }
        public bool IsGroupOpenForAssignment { get; set; }
        public byte Quota { get; set; }
        public decimal ExtraPrice { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NewPrice { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? OldPrice { get; set; }

        public virtual List<Bridge_GroupStudent> GroupStudents { get; set; }
        public virtual List<GroupLessonDay> GroupLessonDays { get; set; }
        public virtual List<GroupExpense> GroupExpenses { get; set; }
    }
}
