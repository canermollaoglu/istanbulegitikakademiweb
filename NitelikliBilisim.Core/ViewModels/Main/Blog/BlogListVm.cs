using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Blog
{
    public class BlogListVm
    {
        public List<BlogCategoryNameIdVm> Categories { get; set; }
        public List<LastBlogPostVm> LastBlogPosts { get; set; }
        public BlogsVm Blogs { get; set; }
        public int TotalBlogPostCount { get; set; }
    }
}
