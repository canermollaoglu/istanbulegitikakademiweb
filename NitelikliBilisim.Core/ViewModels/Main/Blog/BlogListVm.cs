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
        public int TotalBlogPostCount { get; set; }
        public string CurrentCategorySeoUrl { get; set; }
        public string CurrentCategory { get; set; }
        public string SearchKey { get; set; }
    }
}
