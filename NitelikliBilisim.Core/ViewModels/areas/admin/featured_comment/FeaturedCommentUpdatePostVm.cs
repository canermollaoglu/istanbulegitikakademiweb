using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.featured_comment
{
    public class FeaturedCommentUpdatePostVm
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Ad alanı boş olamaz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyad alanı boş olamaz")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Başlık alanı boş olamaz")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Yorum alanı boş olamaz")]
        public string Comment { get; set; }

        public string VideoUrl { get; set; }
        public _PostedFileNotRequired PreviewImageFile { get; set; }
    }
    public class _PostedFileNotRequired
    {
        public string Base64Content { get; set; }
        public string Extension { get; set; }
    }
}
