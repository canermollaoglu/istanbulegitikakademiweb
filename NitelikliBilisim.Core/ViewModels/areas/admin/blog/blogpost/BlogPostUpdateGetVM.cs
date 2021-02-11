using NitelikliBilisim.Core.Entities.blog;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost
{
    public class BlogPostUpdateGetVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public BlogCategory Category { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
        public List<BlogTag> Tags { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string SummaryContent { get; set; }
        public object SeoUrl { get; set; }
        public List<BannerAd> BannerAds { get; set; }
    }
}
