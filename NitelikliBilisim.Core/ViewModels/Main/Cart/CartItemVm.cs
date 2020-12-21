using System;

namespace NitelikliBilisim.Core.ViewModels.Cart
{
    public class CartItemVm
    {
        public Guid EducationId { get; set; }
        public string PreviewPhoto { get; set; }
        public string EducationName { get; set; }
        public decimal PriceNumeric { get; set; }
        public string PriceText { get; set; }
        public decimal OldPriceNumeric { get; set; }
        public object OldPriceText { get; set; }
    }
}
