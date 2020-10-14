using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.promotion
{
    [Table("EducationPromotionConditions")]
    public class EducationPromotionConditions:BaseEntity<Guid>
    {
        public EducationPromotionConditions()
        {
            Id = Guid.NewGuid();
        }

        public ConditionType ConditionType { get; set; }
        public string ConditionValue { get; set; }

    }
}
