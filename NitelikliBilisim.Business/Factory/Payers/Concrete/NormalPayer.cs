using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Sales;
using System;

namespace NitelikliBilisim.Business.PaymentFactory
{
    public class NormalPayer : IPayer
    {
        private readonly IPaymentService _paymentService;
        public NormalPayer(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public PaymentModel Pay(UnitOfWork unitOfWork, PayData data)
        {
            throw new NotImplementedException();
        }
    }
}
