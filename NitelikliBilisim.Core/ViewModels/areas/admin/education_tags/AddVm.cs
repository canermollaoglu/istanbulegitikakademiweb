using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_tags
{
    public class AddGetVm
    {
        public List<EducationTag> Tags { get; set; }
    }

    public class AddPostVm
    {
        [Required(ErrorMessage = "Kategori ismi boş geçilemez"), MaxLength(100, ErrorMessage = "Kategori ismi en fazla 100 karakter olabilir")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama alanı boş geçilemez"), MaxLength(300, ErrorMessage = "Açıklama alanı en fazla 300 karakter içerebilir")]
        public string Description { get; set; }
        public Guid? BaseTagId { get; set; }
    }
}
