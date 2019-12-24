using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationParts")]
    public class EducationPart : BaseEntity<Guid>
    {
        public EducationPart()
        {
            Id = Guid.NewGuid();
        }
        [MaxLength(128)]
        public string Title { get; set; }
        public byte Duration { get; set; }
        public byte Order { get; set; }

        [ForeignKey("Education")]
        public Guid EducationId { get; set; }
        public virtual Education Education { get; set; }
    }
}
