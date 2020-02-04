using Iyzipay.Model;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Sales;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.Services.Payments
{
    public interface IPaymentService
    {
        ThreedsInitialize MakePayment(PayPostVm data, ApplicationUser user, List<Education> cartItems);
    }
}
