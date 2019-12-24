﻿using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationCategories")]
    public class EducationCategory : BaseEntity<Guid>
    {
        public EducationCategory()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }
        public bool IsCurrent { get; set; }

        public Guid? BaseCategoryId { get; set; }
        [ForeignKey("BaseCategoryId")]
        public virtual EducationCategory BaseCategory { get; set; }
    }
}