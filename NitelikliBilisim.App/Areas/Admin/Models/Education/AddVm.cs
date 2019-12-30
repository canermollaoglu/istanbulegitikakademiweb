using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Models.Education
{
    public class AddGetVm
    {
        public List<EducationTag> Tags { get; set; }
        public List<EducationCategory> Categories { get; set; }
        public Dictionary<int, string> Levels { get; set; }
    }

    public class EducationCrudVm
    {
        [Required(ErrorMessage = "İsim alanı boş geçilemez"), MaxLength(100, ErrorMessage = "Eğitim ismi 100 karakterden fazla olamaz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama alanı boş geçilemez"), MaxLength(500, ErrorMessage = "Açıklama alanı 500 karakterden fazla olamaz")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Açıklama (2) alanı boş geçilemez"), MaxLength(500, ErrorMessage = "Açıklama (2) alanı 500 karakterden fazla olamaz")]
        public string Description2 { get; set; }
        [Required(ErrorMessage = "Fiyat alanı boş geçilemez")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Eğitimin kaç gün süreceği bilgisi boş geçilemez"), Range(1, 255, ErrorMessage = "Eğitim günü 1 günden daha az olamaz")]
        public byte? Days { get; set; }
        [Required(ErrorMessage = "Eğitim günde kaç saat işleneceği bilgisi boş geçilemez"), Range(1, 24, ErrorMessage = "Günlük işlenecek ders saati 1 saatten az olamaz")]
        public byte? HoursPerDay { get; set; }
        [Required(ErrorMessage = "Eğitim seviyesi seçilmelidir")]
        public int? EducationLevel { get; set; }
        [Required(ErrorMessage = "Eğitim en az 1 kategoriye ait olmalıdır")]
        public Guid CategoryId { get; set; }
        public List<Guid> TagIds { get; set; }
    }
    public class AddPostVm : EducationCrudVm
    {
        public _PostedFile BannerFile { get; set; }
        public _PostedFile PreviewFile { get; set; }
    }

    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
