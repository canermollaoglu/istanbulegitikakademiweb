using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.bannerAd
{
    public class AddBannerAdVm
    {
        [Required]
        public string Title1 { get; set; }
        [Required]
        public string Title2 { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string IconUrl { get; set; }
        [Required]
        public string Content { get; set; }
        public string RelatedApplicationUrl { get; set; }
        [Required]
        public _PostedFile FeaturedImage { get; set; }
    }
    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
