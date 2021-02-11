using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Sales;
using System;

namespace NitelikliBilisim.Business.PaymentFactory
{
    public class BkmPayer : IPayer
    {
        private readonly IPaymentService _service;
        public BkmPayer(IPaymentService service)
        {
            _service = service;
        }
        public PaymentModel Pay(UnitOfWork unitOfWork, PayData data)
        {
            throw new NotImplementedException();
        }
    }
}
