using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class StudentUsedPromotionListVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UsedDate { get; set; }
        public decimal DiscountAmount { get; set; }
        public Guid InvoiceId { get; set; }
        public string PromotionCode { get; set; }
        public PromotionType PromotionType { get; set; }
    }
}
