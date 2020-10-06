using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities.educations
{
    public class EducationPromotionItem
    {
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid InvoiceId { get; set; }

        [ForeignKey("EducationPromotionCodeId")]
        public Guid EducationPromotionCodeId { get; set; }
        public virtual EducationPromotionCode EducationPromotionCode { get; set; }
    }
}
