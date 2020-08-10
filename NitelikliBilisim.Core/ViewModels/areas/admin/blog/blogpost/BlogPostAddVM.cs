using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost
{
    public class BlogPostAddVM
    {
        [Required(ErrorMessage = "Başlık alanı boş geçilemez"), MaxLength(128, ErrorMessage = "Eğitim ismi 128 karakterden fazla olamaz")]
        public string Title { get; set; }
        [Required(ErrorMessage = "İçerik alanı boş geçilemez")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Kategori alanı boş geçilemez")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Etiket alanı boş geçilemez")]
        public string[] Tags { get; set; }
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
