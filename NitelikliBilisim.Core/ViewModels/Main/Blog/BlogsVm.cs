using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Blog
{
    public class BlogsVm
    {
        public List<BlogPostListVm> Posts { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
    }
}
