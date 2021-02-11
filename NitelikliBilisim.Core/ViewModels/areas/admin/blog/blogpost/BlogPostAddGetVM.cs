using NitelikliBilisim.Core.Entities.blog;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost
{
    public class BlogPostAddGetVM
    {
        public List<BlogCategory> BlogCategories{ get; set; }
        public List<BannerAd> BannerAds { get; set; }
    }
}
