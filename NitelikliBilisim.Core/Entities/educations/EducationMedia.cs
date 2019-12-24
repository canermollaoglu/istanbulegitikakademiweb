using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationMediaItems")]
    public class EducationMedia : BaseEntity<Guid>
    {
        public EducationMedia()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(256)]
        public string FileUrl { get; set; }
        public EducationMediaType MediaType { get; set; }

        [ForeignKey("Education")]
        public Guid EducationId { get; set; }
        public virtual Education Education { get; set; }
    }
}
