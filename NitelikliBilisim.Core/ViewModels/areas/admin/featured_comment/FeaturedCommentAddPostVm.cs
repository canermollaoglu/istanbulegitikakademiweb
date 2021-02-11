using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.featured_comment
{
    public class FeaturedCommentAddPostVm
    {
        [Required(ErrorMessage = "Ad alanı boş olamaz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyad alanı boş olamaz")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Başlık alanı boş olamaz")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Yorum alanı boş olamaz")]
        public string Comment { get; set; }

        public string VideoUrl { get; set; }
        public _PostedFile PreviewImageFile { get; set; }

    }

    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
