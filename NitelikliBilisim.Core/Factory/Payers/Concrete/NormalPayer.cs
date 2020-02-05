using NitelikliBilisim.Core.Services.Payments;
using System;

namespace NitelikliBilisim.Core.Factory.Payers.Concrete
{
    public class NormalPayer : INormalPayer
    {
        private readonly IPaymentService _paymentService;
        public NormalPayer(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public PaymentModel Pay()
        {
            throw new NotImplementedException();
        }
    }
}
