using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.blog
{
    [Table("BlogPosts")]
    public class BlogPost : BaseEntity<Guid>
    {
        [MaxLength(128)]
        public string Title { get; set; }
        public string Content { get; set; }
        public string SummaryContent { get; set; }
        public string FeaturedImageUrl { get; set; }
        public int ReadingTime { get; set; }
        public bool IsActive { get; set; }
        public string SeoUrl { get; set; }
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual BlogCategory Category { get; set; }
    }
}
