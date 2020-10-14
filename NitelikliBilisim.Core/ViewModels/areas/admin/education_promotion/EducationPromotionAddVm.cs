using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion
{
    public class EducationPromotionAddVm
    {
        [MaxLength(150), Required(ErrorMessage = "Promosyon adı boş geçilemez.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Başlangıç tarihi alanı boş geçilemez.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Bitiş tarihi alanı boş geçilemez.")]
        public DateTime EndDate { get; set; }
        public string PromotionCode { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Maksimum kullanım adedi adı boş geçilemez.")]
        public int MaxUsageLimit { get; set; }
        [Required(ErrorMessage = "Kullanıcı bazlı maksimum kullanım adedi adı boş geçilemez.")]
        public int UserBasedUsageLimit { get; set; }
        [Required(ErrorMessage = "Promosyon tutarı alanı boş geçilemez.")]
        public decimal DiscountAmount { get; set; }
        [Required(ErrorMessage = "Minimum sepet tutarı boş geçilemez.")]
        public decimal MinBasketAmount { get; set; }
    }
}
