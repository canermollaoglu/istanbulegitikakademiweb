using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Educations")]
    public class Education : BaseEntity<Guid>
    {
        public Education()
        {
            Id = Guid.NewGuid();
        }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        [MaxLength(512)]
        public string Description2 { get; set; }
        [MaxLength(512)]
        public string Description3 { get; set; }
        public byte Days { get; set; }
        public byte HoursPerDay { get; set; }
        public EducationLevel Level { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(128)]
        public string SeoUrl { get; set; }
        [MaxLength(128)]
        public string VideoUrl { get; set; }
        public bool IsFeaturedEducation { get; set; }
        public byte Order { get; set; }
        /// <summary>
        /// Bu field kullanıcıya haftalık eğitim önerilerinde kullanılmak üzere oluşturuldu. 
        /// Önerilerde bu eğitimin önerileceği kişinin fieldde belirtilen kategorilerden birinde olması şartı arandı.
        /// </summary>
        public string RelatedNBUYCategories { get; set; }

        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual EducationCategory Category { get; set; }

        public virtual List<EducationSuggestionCriterion> EducationSuggestionCriterions { get; set; }
    }
}
