using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities.educations
{
    public class EducationPromotionItem : IEntity<Guid>
    {
        public EducationPromotionItem()
        {
           Id = Guid.NewGuid();
        }
        [Key]
        [Column(Order = 1)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid InvoiceId { get; set; }

        [ForeignKey("EducationPromotionCode")]
        public Guid EducationPromotionCodeId { get; set; }
        public virtual EducationPromotionCode EducationPromotionCode { get; set; }
    }
}
