using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.Core.PaymentModels
{
    public class CartItem
    {
        public EducationGroup EducationGroup { get; set; }
        public Guid InvoiceDetailsId { get; set; }
    }
}
