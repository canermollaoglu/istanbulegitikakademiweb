using NitelikliBilisim.Core.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.Factory.Payers.Concrete
{
    public class Secure3dPayer : ISecure3dPayer
    {
        private readonly IPaymentService _service;
        public Secure3dPayer(IPaymentService service)
        {
            _service = service;
        }
        public PaymentModel Pay()
        {
            throw new NotImplementedException();
        }
    }
}
