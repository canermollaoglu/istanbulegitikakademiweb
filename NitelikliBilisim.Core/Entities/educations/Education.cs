using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Educations")]
    public class Education : BaseEntity<Guid>
    {
        public Education()
        {
            Id = Guid.NewGuid();
        }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NewPrice { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? OldPrice { get; set; }
        public byte Days { get; set; }
        public byte HoursPerDay { get; set; }
        public EducationLevel Level { get; set; }
        public bool IsActive { get; set; }
    }
}
