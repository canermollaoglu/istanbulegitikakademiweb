using Iyzipay.Model;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Sales;
using System;

namespace NitelikliBilisim.Business.PaymentFactory
{
    public class Secure3dPayer : IPayer
    {
        private readonly IPaymentService _service;
        public Secure3dPayer(IPaymentService service)
        {
            _service = service;
        }
        public PaymentModel Pay(UnitOfWork unitOfWork, PayData data)
        {
            var user = unitOfWork.Sale.GetUser(data.SpecialInfo.UserId);
            var cartItems = unitOfWork.Sale.Sell(data, user.Id);
            var paymentResult = _service.Make3DsPayment(data, user, cartItems);

            var result = new PaymentModel
            {
                TransactionType = TransactionType.Secure3d,
                ConversationId = paymentResult.ConversationId,
                HtmlContent = paymentResult.HtmlContent,
                Locale = paymentResult.Locale,
                Status = paymentResult.Status
            };
            if (!IsValidConversation(data.ConversationId, paymentResult))
                result.Error = new PaymentModelError
                {
                    ErrorCode = paymentResult.ErrorCode,
                    ErrorGroup = paymentResult.ErrorGroup,
                    ErrorMessage = paymentResult.ErrorMessage
                };

            return result;
        }
        public bool IsValidConversation(Guid determinedConversationId, ThreedsInitialize paymentResult)
        {
            if (paymentResult.Status == "success"
                && determinedConversationId.ToString() == paymentResult.ConversationId
                && paymentResult.ErrorCode == null
                && paymentResult.ErrorMessage == null
                && paymentResult.ErrorGroup == null
                && paymentResult.HtmlContent != null)
                return true;
            return false;
        }
    }
}
