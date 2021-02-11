using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("BlogSubscribers")]
    public class BlogSubscriber : BaseEntity<Guid>
    {
        public BlogSubscriber()
        {
            Id = Guid.NewGuid();
        }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [Required, MaxLength(150)]
        public string Email { get; set; }

        public bool IsCanceled { get; set; }
    }
}
