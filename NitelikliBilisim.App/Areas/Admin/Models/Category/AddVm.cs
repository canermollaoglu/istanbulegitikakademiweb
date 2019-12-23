﻿using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.App.Areas.Admin.Models.Category
{
    public class AddGetVm
    {
        public List<EducationCategory> Categories { get; set; }
    }

    public class AddPostVm
    {
        [Required(ErrorMessage = "Kategori ismi boş geçilemez"), MaxLength(100, ErrorMessage = "Kategori ismi en fazla 100 karakter olabilir")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama alanı boş geçilemez"), MaxLength(300, ErrorMessage = "Açıklama alanı en fazla 300 karakter içerebilir")]
        public string Description { get; set; }
        public Guid? BaseCategoryId { get; set; }
        public int CategoryType { get; set; }
    }
}
