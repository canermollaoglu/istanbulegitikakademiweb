using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("StudentEducationInfos")]
    public class StudentEducationInfo : BaseEntity<Guid>
    {
        public StudentEducationInfo()
        {
            Id = Guid.NewGuid();
        }

        public EducationCenter EducationCenter { get; set; }
        public DateTime StartedAt { get; set; }
        public Guid? CategoryId { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual List<EducationDay> EducationDays { get; set; }
    }
}
