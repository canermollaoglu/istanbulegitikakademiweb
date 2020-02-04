using Iyzipay.Model;
using Iyzipay.Request;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Sales;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.Services.Payments
{
    public interface IPaymentService
    {
        InstallmentInfo CheckInstallment(string conversationId, string binNumber, decimal price);
        ThreedsInitialize Make3DsPayment(PayPostVm data, ApplicationUser user, List<Education> cartItems);
        ThreedsPayment Confirm3DsPayment(CreateThreedsPaymentRequest request);
        Payment MakePayment(PayPostVm data, ApplicationUser user, List<Education> cartItems);
        Payment CheckPayment(RetrievePaymentRequest request);
        Cancel CreateCancelRequest(string conversationId, string paymentId, string ip, RefundReason reason, string description);
        Refund CreateRefundRequest(string conversationId, string paymentTransactionId, decimal price, string ip, RefundReason reason, string description);
        BkmInitialize MakeBkmPayment(PayPostVm data, ApplicationUser user, List<Education> cartItems);
        Bkm ConfirmBkmPayment(RetrieveBkmRequest request);
    }
}
