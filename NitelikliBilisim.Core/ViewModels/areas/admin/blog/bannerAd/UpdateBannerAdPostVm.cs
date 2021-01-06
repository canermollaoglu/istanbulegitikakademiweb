using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.bannerAd
{
    public class UpdateBannerAdPostVm
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Başlık 1 alanı zorunludur.")]
        public string Title1 { get; set; }
        [Required(ErrorMessage = "Başlık 2 alanı zorunludur.")]
        public string Title2 { get; set; }
        [Required(ErrorMessage = "Kod alanı zorunludur.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "İkon Url alanı zorunludur.")]
        public string IconUrl { get; set; }
        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        public string Content { get; set; }
        public string RelatedApplicationUrl { get; set; }
        public _PostedFileNotRequired FeaturedImage { get; set; }
    }
    public class _PostedFileNotRequired
    {
        public string Base64Content { get; set; }
        public string Extension { get; set; }
    }

}
