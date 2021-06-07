using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.popular_topic
{
    public class PopularTopicAddPostVm
    {
        [Required(ErrorMessage = "Kısa başlık alanı boş olamaz")]
        public string ShortTitle { get; set; }
        [Required(ErrorMessage = "Başlık alanı boş olamaz")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Açıklama alanı boş olamaz")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Hedef URL alanı boş olamaz")]
        public string TargetUrl { get; set; }
        [Required(ErrorMessage = "Bağlantılı kategori alanı boş olamaz")]
        public Guid RelatedCategory { get; set; }

        public _PostedFile IconImage { get; set; }
        public _PostedFile BackgroundImage { get; set; }
    }
    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
