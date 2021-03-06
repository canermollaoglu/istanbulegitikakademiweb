using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("NewsletterSubscribers")]
    public class NewsletterSubscriber :BaseEntity<Guid>
    {
        public NewsletterSubscriber()
        {
            Id = Guid.NewGuid();
        }
        [Required, MaxLength(150)]
        public string Email { get; set; }
        public bool IsCanceled { get; set; }
    }
}
