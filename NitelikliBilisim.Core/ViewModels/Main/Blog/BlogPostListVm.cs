using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Blog
{
    public class BlogPostListVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedDate { get; set; }
        public string Category { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string ReadingTime { get; set; }
        public string SeoUrl { get; set; }
        public string CategorySeoUrl { get; set; }
        public int ViewCount { get; set; }
    }
}
