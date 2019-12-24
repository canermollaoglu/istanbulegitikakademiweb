using NitelikliBilisim.Core.Abstracts;
using System;
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
        public DateTime ValidThru { get; set; }
        [MaxLength(7)]
        public string Code { get; set; }
        public byte OffPercentage { get; set; }
        public bool IsUsed { get; set; }
        public byte TimesUsable { get; set; }
    }
}
