using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EmailTemplates")]
    public class EmailTemplate : BaseEntity<Guid>
    {
        public EmailTemplate()
        {
            Id = Guid.NewGuid();
        }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }

        public EmailTemplateType Type{ get; set; }
    }
}
