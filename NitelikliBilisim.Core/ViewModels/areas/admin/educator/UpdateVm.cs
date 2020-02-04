﻿using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator
{
    public class UpdateGetVm : AddGetVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Biography { get; set; }
    }

    public class UpdatePostVm : AddPostVm
    {
        public Guid EducatorId { get; set; }
    }

    public class UpdatePostNewVm
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
        public Guid EducatorId { get; set; }
    }

    public class UpdateGetEducatorSocialMediaVm : AddGetVm
    {
        public Guid Id { get; set; }
        public string Facebook { get; set; }
        public string Linkedin { get; set; }
        public string GooglePlus { get; set; }
        public string Twitter { get; set; }
    }

    public class UpdatePostEducatorSocialMediaVm
    {
        [MaxLength(100, ErrorMessage = "Facebook linki en fazla 100 karakter içerebilir")]
        public string Facebook { get; set; }
        [MaxLength(100, ErrorMessage = "Linkedin linki en fazla 100 karakter içerebilir")]
        public string Linkedin { get; set; }
        [MaxLength(100, ErrorMessage = "Google Plus linki en fazla 100 karakter içerebilir")]
        public string GooglePlus { get; set; }
        [MaxLength(100, ErrorMessage = "Twitter linki en fazla 100 karakter içerebilir")]
        public string Twitter { get; set; }
        public string EducatorId { get; set; }
    }
}
