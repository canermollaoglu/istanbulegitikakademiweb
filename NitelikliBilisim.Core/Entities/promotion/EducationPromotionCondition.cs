using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.promotion
{
    [Table("EducationPromotionConditions")]
    public class EducationPromotionCondition:BaseEntity<Guid>
    {
        public EducationPromotionCondition()
        {
            Id = Guid.NewGuid();
        }

        public ConditionType ConditionType { get; set; }
        public string ConditionValue { get; set; }

        [ForeignKey("EducationPromotionCode")]
        public Guid EducationPromotionCodeId { get; set; }
        public virtual EducationPromotionCode EducationPromotionCode { get; set; }

    }
}
