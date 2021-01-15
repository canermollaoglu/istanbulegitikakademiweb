using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
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
        [MaxLength(128)]
        public string SeoUrl { get; set; }
        public string IconUrl { get; set; }
        public string WizardClass { get; set; }
        public string IconColor { get; set; }
        /// <summary>
        /// NBUY Eğitimleri için eğitim gün sayısı
        /// </summary>
        public int? EducationDayCount{ get; set; }

        public Guid? BaseCategoryId { get; set; }
        [ForeignKey("BaseCategoryId")]
        public virtual EducationCategory BaseCategory { get; set; }
        public virtual List<Education> Educations { get; set; }
    }
}
