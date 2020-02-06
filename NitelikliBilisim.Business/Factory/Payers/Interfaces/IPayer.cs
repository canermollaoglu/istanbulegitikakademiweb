using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.ViewModels.Sales;

namespace NitelikliBilisim.Business.PaymentFactory
{
    public interface IPayer
    {
        PaymentModel Pay(UnitOfWork unitOfWork, PayData data);
    }
}
