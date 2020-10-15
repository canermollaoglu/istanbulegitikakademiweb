using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion
{
    public class EducationPromotionUpdateGetVm
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public int UserBasedUsageLimit { get; set; }
        public decimal MinBasketAmount { get; set; }
        public string PromotionCode { get; set; }
        public int MaxUsageLimit { get; set; }
        public string Name { get; set; }
        public PromotionType PromotionType { get; set; }
    }
}
