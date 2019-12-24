using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationTags")]
    public class EducationTag : BaseEntity<Guid>
    {
        public EducationTag()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }

        public Guid? BaseTagId { get; set; }
        [ForeignKey("BaseTagId")]
        public virtual EducationTag BaseTag { get; set; }
    }
}
