using Iyzipay.Model;
using Iyzipay.Request;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.ViewModels.Sales;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.Services.Payments
{
    public interface IPaymentService
    {
        InstallmentInfo CheckInstallment(string conversationId, string binNumber, decimal price);
        ThreedsInitialize Make3DsPayment(PayData data, ApplicationUser user, List<CartItem> cartItems);
        ThreedsPayment Confirm3DsPayment(CreateThreedsPaymentRequest request);
        Payment MakePayment(PayData data, ApplicationUser user, List<CartItem> cartItems);
        Payment CheckPayment(RetrievePaymentRequest request);
        Cancel CreateCancelRequest(string conversationId, string paymentId, string ip, RefundReason reason, string description);
        Refund CreateRefundRequest(string conversationId, string paymentTransactionId, decimal price, string ip, RefundReason reason, string description);
        BkmInitialize MakeBkmPayment(PayData data, ApplicationUser user, List<CartItem> cartItems);
        Bkm ConfirmBkmPayment(RetrieveBkmRequest request);

        CheckoutFormInitialize MakeCheckoutForm(PayData data, ApplicationUser user,
            List<CartItem> cartItems);

        CheckoutForm ConfirmCheckoutForm(RetrieveCheckoutFormRequest request);
    }
}