using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.popular_topic
{
    public class PopularTopicUpdatePostVm
    {
        public Guid Id { get; set; }
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

        public _NotRequiredPostedFile IconImage { get; set; }
        public _NotRequiredPostedFile BackgroundImage { get; set; }
    }
    public class _NotRequiredPostedFile
    {
        public string Base64Content { get; set; }
        public string Extension { get; set; }
    }
}
