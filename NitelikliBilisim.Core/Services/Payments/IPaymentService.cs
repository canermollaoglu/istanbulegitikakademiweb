using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Sales;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.Services.Payments
{
    public interface IPaymentService
    {
        Iyzipay.Model.Payment MakePayment(PayPostVm data, ApplicationUser user, List<Education> cartItems);
    }
}
