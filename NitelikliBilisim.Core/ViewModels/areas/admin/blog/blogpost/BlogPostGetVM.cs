using NitelikliBilisim.Core.Entities.blog;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost
{
    public class BlogPostGetVM
    {
        public Guid Id { get; set; }
        public BlogCategory Category { get; set; }
        public List<BlogTag> Tags { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FeaturedImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReadingTime { get; set; }

    }
}
