using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.educations
{
    [Table("EducationHostImages")]
   public class EducationHostImage : BaseEntity<Guid>
    {
        public EducationHostImage()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(256)]
        public string FileUrl { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("EducationHost")]
        public Guid EducationHostId { get; set; }
        public virtual EducationHost EducationHost { get; set; }

    }
}
