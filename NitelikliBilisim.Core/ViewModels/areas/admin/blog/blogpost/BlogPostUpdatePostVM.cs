using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost
{
    public class BlogPostUpdatePostVM
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Başlık alanı boş geçilemez"), MaxLength(128, ErrorMessage = "Eğitim ismi 128 karakterden fazla olamaz")]
        public string Title { get; set; }
        [Required(ErrorMessage = "İçerik alanı boş geçilemez")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Kategori alanı boş geçilemez")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Etiket alanı boş geçilemez")]
        public string[] Tags { get; set; }
        public _PostedFileNotRequired FeaturedImage { get; set; }

        [Required(ErrorMessage = "Özet bilgisi boş geçilemez")]
        public string SummaryContent { get; set; }
    }
    public class _PostedFileNotRequired
    {
        public string Base64Content { get; set; }
        public string Extension { get; set; }
    }
}
