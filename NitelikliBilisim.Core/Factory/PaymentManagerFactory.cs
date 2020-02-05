using NitelikliBilisim.Core.Factory.Enums;
using NitelikliBilisim.Core.Factory.Payers;
using NitelikliBilisim.Core.Factory.Payers.Concrete;
using NitelikliBilisim.Core.Services.Payments;

namespace NitelikliBilisim.Core.Factory
{
    public class PaymentManagerFactory
    {
        private readonly IPaymentService _service;
        public PaymentManagerFactory(IPaymentService service)
        {
            _service = service;
        }
        public IPayer Create(TransactionType type)
        {
            switch (type)
            {
                case TransactionType.Normal:
                    return new NormalPayer(_service);
                case TransactionType.Secure3d:
                    return new Secure3dPayer(_service);
                case TransactionType.BKM:
                    return new BkmPayer(_service);
                default:
                    return new NormalPayer(_service);
            }
        }
    }
}
