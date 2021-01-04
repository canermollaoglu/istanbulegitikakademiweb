using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.App.Models.Account
{
    public class RegisterGetVm
    {
        public Dictionary<int, string> EducationCenters { get; set; }
        public List<EducationCategory> EducationCategories { get; set; }
    }

    public class RegisterPostVm
    {
        [Required(ErrorMessage = "isim alanı gereklidir.")]
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim alanı gereklidir.")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-Posta alanı gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Phone { get; set; }

        [Required(ErrorMessage = "Şifre tekrar alanı gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
        public string ConfirmPassword { get; set; }

        public bool IsNbuyStudent { get; set; }
        public int EducationCenter { get; set; }
        public Guid? EducationCategory { get; set; }
        public string StartedAt { get; set; }
        public bool AcceptedTerms { get; set; }
    }
}
