using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Blog
{
    public class LastBlogPostVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CreatedDate { get; set; }
        public string Category { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string ReadingTime { get; set; }
    }
}
