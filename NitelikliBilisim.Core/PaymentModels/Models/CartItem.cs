using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.Core.PaymentModels
{
    public class CartItem
    {
        public Guid InvoiceDetailsId { get; set; }
        public Education Education { get; set; }
    }
}
