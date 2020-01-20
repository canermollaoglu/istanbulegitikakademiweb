using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Cart
{
    public class CartItem
    {
        public Guid EducationId { get; set; }
        public string PreviewPhoto { get; set; }
        public string EducationName { get; set; }
        public decimal PriceNumeric { get; set; }
        public string PriceText { get; set; }
    }
}
