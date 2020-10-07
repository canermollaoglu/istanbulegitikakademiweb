using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.educations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationPromotionCodes")]
    public class EducationPromotionCode : BaseEntity<Guid>
    {
        public EducationPromotionCode()
        {
            Id = Guid.NewGuid();
        }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(7)]
        public string PromotionCode { get; set; }
        public string Description { get; set; }
        public int MaxUsageLimit { get; set; }
        public int UserBasedUsageLimit { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinBasketAmount { get; set; }
        public virtual List<EducationPromotionItem> EducationPromotionItems { get; set; }
    }
}
