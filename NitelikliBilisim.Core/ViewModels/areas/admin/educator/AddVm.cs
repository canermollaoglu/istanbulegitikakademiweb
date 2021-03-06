using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.user_details;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator
{
    public class AddGetVm
    {

        public List<EducatorCertificate> Certificates { get; set; }
        public Dictionary<int,string> BankNames { get; set; }
        public List<EducationCategory> EducationCategories { get; set; }
    }

    public class AddPostVm
    {
        [Required(ErrorMessage = "İsim alanı boş geçilemez"), MaxLength(30, ErrorMessage = "İsim alanı en fazla 30 karakter içerebilir")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim alanı boş geçilemez"), MaxLength(30, ErrorMessage = "Soyisim alanı en fazla 30 karakter içerebilir")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Telefon alanı boş geçilemez"), MinLength(10, ErrorMessage = "Telefon alanı en az 10 karakter içerebilir"), MaxLength(11, ErrorMessage = "Telefon alanı en fazla 11 karakter içerebilir")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "E-Posta alanı boş geçilemez"), EmailAddress(ErrorMessage = "E-Posta alanı, e-posta formatına uygun değildir")]
        public string Email { get; set; }
        public string Title { get; set; }
        public _SocialMedia SocialMedia { get; set; }
        public _PostedFile ProfilePhoto { get; set; }
        [Required(ErrorMessage = "Biyografi alanı boş geçilemez")]
        public string Biography { get; set; }
        [Required(ErrorMessage = "Kısa Açıklama alanı boş geçilemez"),MaxLength(400,ErrorMessage ="Kısa Açıklama alanı en fazla 400 karakter içerebilir.")]
        public string ShortDescription { get; set; }
        public List<int> CertificateIds { get; set; }
        public List<Guid> EducatorCategoryIds { get; set; }
        public int Bank { get; set; }
        [MaxLength(26,ErrorMessage ="IBAN alanı en fazla 26 karakter içerebilir.")]
        public string IBAN { get; set; }

    }

    public class _SocialMedia
    {
        [MaxLength(100, ErrorMessage = "Facebook linki en fazla 100 karakter içerebilir")]
        public string Facebook { get; set; }
        [MaxLength(100, ErrorMessage = "Linkedin linki en fazla 100 karakter içerebilir")]
        public string Linkedin { get; set; }
        [MaxLength(100, ErrorMessage = "Google Plus linki en fazla 100 karakter içerebilir")]
        public string GooglePlus { get; set; }
        [MaxLength(100, ErrorMessage = "Twitter linki en fazla 100 karakter içerebilir")]
        public string Twitter { get; set; }
    }
    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
