using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion
{
    public class EducationBasketBasedPromotionListVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int MaxUsageLimit { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinBasketAmount { get; set; }
        public string IsActive { get; set; }
        public int CountOfUses { get; set; }
        public int UserBasedUsageLimit { get; set; }
        public PromotionType PromotionType { get; set; }
    }
}
