using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.App.Areas.Admin.Models.Category
{
    public class AddGetVm
    {
        public List<EducationCategory> Categories { get; set; }
        public Dictionary<int, string> Types { get; set; }
    }

    public class AddPostVm
    {
        [Required(ErrorMessage = "Kategori ismi boş geçilemez"), MaxLength(100, ErrorMessage = "Kategori ismi en fazla 100 karakter olabilir")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama alanı boş geçilemez"), MaxLength(300, ErrorMessage = "Açıklama alanı en fazla 300 karakter içerebilir")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Kategori için bir Seo Url girmelisiniz.")]
        public string SeoUrl { get; set; }
        [Required(ErrorMessage ="Sıra bilgisi boş geçilemez")]
        public byte Order { get; set; }
        public string IconUrl { get; set; }
        public string WizardClass { get; set; }
        public string Description2 { get; set; }
        public string IconColor { get; set; }
        public Guid? BaseCategoryId { get; set; }
        public int? EducationDayCount { get; set; }
        public int CategoryType { get; set; }
      
        public _PostedFile IconImage { get; set; }
        public _PostedFile BackgroundImage { get; set; }
    }
    public class _PostedFile
    {
        public string Base64Content { get; set; }
        public string Extension { get; set; }
    }
}
