using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationPhotoGallery")]
    public class EducationPhotoGallery : BaseEntity<Guid>
    {
        public EducationPhotoGallery()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(256)]
        public string PhotoUrl { get; set; }
        public EducationPhotoType PhotoType { get; set; }

        [ForeignKey("Education")]
        public Guid EducationId { get; set; }
        public virtual Education Education { get; set; }
    }
}
