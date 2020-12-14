using System;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost
{
    public class BlogPostListVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReadingTime { get; set; }
    }
}
