using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.blog
{
    [Table("BlogTags")]
    public class BlogTag:BaseEntity<Guid>
    {
        public BlogTag()
        {
            Id = Guid.NewGuid();
        }
        public string Name { get; set; }
    }
}
