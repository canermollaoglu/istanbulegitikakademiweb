using NitelikliBilisim.Core.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.Factory.Payers.Concrete
{
    public class BkmPayer : IPayer
    {
        private readonly IPaymentService _service;
        public BkmPayer(IPaymentService service)
        {
            _service = service;
        }
        public PaymentModel Pay()
        {
            throw new NotImplementedException();
        }
    }
}
