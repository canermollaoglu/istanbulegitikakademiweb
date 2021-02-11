using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.bannerAd
{
    public class UpdateBannerAdGetVm
    {
        public Guid Id { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public string RelatedApplicationUrl { get; set; }
        public string Code { get; set; }
    }
}
