using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Blog
{
    public class BlogPostGetDetailVm
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Tags { get; set; }
    }
}
