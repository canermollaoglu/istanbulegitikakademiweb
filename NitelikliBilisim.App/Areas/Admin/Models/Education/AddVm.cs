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
        public List<EducationCategory> Categories { get; set; }
        public Dictionary<int, string> Levels { get; set; }
        public List<EducationCategory> BaseCategories { get; internal set; }
    }

    public class EducationCrudVm
    {
        [Required(ErrorMessage = "İsim alanı boş geçilemez"), MaxLength(100, ErrorMessage = "Eğitim ismi 100 karakterden fazla olamaz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama alanı boş geçilemez"), MaxLength(500, ErrorMessage = "Açıklama alanı 500 karakterden fazla olamaz")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Açıklama (2) alanı boş geçilemez"), MaxLength(500, ErrorMessage = "Açıklama (2) alanı 500 karakterden fazla olamaz")]
        public string Description2 { get; set; }

        [Required(ErrorMessage = "Açıklama (3) alanı boş geçilemez"), MaxLength(500, ErrorMessage = "Açıklama (3) alanı 500 karakterden fazla olamaz")]
        public string Description3 { get; set; }

        [Required(ErrorMessage = "Eğitimin kaç gün süreceği bilgisi boş geçilemez"), Range(1, 255, ErrorMessage = "Eğitim günü 1 günden daha az olamaz")]
        public byte? Days { get; set; }
        [Required(ErrorMessage = "Eğitim günde kaç saat işleneceği bilgisi boş geçilemez"), Range(1, 24, ErrorMessage = "Günlük işlenecek ders saati 1 saatten az olamaz")]
        public byte? HoursPerDay { get; set; }
        [Required(ErrorMessage = "Eğitim seviyesi seçilmelidir")]
        public int? EducationLevel { get; set; }
        [Required(ErrorMessage = "Eğitim en az 1 kategoriye ait olmalıdır")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage ="Eğitim için bir Seo Url girmelisiniz.")]
        public string SeoUrl { get; set; }
        [Required(ErrorMessage = "Eğitim için bir sıra girmelisiniz.")]
        public byte Order{ get; set; }
        public string VideoUrl { get; set; }
        public string[] Tags { get; set; }
        [Required(ErrorMessage = "Eğitimin önerileceği NBUY öğrenci kategorilerini seçmelisiniz.")]
        public string[] SuggestedCategories { get; set; }
    }
    public class AddPostVm : EducationCrudVm
    {
       
    }

    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
    public class _PostedFileUpdate
    {
        public string Base64Content { get; set; }
        public string Extension { get; set; }
    }
}
