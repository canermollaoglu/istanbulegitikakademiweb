using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
    }
}
