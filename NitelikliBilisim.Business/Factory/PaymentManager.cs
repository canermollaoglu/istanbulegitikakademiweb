using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Sales;

namespace NitelikliBilisim.Business.PaymentFactory
{
    public class PaymentManager
    {
        private readonly IPayer _payer;
        public PaymentManager(IPaymentService service, TransactionType transactionType)
        {
            _payer = CreatePayer(service, transactionType);
        }

        public PaymentModel Pay(UnitOfWork unitOfWork, PayData data)
        {
            return _payer.Pay(unitOfWork, data);
        }

        private IPayer CreatePayer(IPaymentService service, TransactionType type)
        {
            switch (type)
            {
                case TransactionType.Normal:
                    return new NormalPayer(service);
                case TransactionType.Secure3d:
                    return new Secure3dPayer(service);
                case TransactionType.BKM:
                    return new BkmPayer(service);
                default:
                    return new NormalPayer(service);
            }
        }
    }
}
