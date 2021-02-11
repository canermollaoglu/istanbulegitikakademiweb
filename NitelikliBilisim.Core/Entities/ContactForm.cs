using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("ContactForms")]
    public class ContactForm :BaseEntity<Guid>
    {
        public ContactForm()
        {
            Id = Guid.NewGuid();
        }
        [Required,MaxLength(150)]
        public string Name { get; set; }
        [Required, MaxLength(150)]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public ContactFormTypes ContactFormType { get; set; }
    }
}
