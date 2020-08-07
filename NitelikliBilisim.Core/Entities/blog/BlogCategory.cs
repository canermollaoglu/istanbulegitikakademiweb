using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.blog
{
    [Table("BlogCategories")]
    public class BlogCategory:BaseEntity<Guid>
    {
        public BlogCategory()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }

        public virtual List<BlogPost> BlogPosts { get; set; }

    }
}
