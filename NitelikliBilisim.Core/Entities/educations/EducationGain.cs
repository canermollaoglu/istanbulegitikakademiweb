using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationGains")]
    public class EducationGain : BaseEntity<Guid>
    {
        public EducationGain()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(512)]
        public string Gain { get; set; }
        public Guid EducationId { get; set; }
        [ForeignKey("EducationId")]
        public virtual Education Education { get; set; }
    }
}
