using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("SaleDetails")]
    public class SaleDetail : BaseEntity<Guid>
    {
        public decimal Price { get; set; }
        public Guid SaleId { get; set; }
        public Guid EducationId { get; set; }
        public Guid? PromotionCodeId { get; set; }
        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }
        [ForeignKey("EducationId")]
        public virtual Education Education { get; set; }
        [ForeignKey("PromotionCodeId")]
        public virtual EducationPromotionCode PromotionCode { get; set; }
    }
}
