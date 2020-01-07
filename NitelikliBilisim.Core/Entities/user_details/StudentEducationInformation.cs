using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("StudentEducationInformations")]
    public class StudentEducationInformation : BaseEntity<Guid>
    {
        public StudentEducationInformation()
        {
            Id = Guid.NewGuid();
        }

        public EducationCenter EducationCenter { get; set; }
        public DateTime StartedAt { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
