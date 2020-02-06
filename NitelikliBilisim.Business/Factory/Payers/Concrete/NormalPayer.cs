using Iyzipay.Model;
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
            var user = unitOfWork.Sale.GetUser(data.SpecialInfo.UserId);
            var cartItems = unitOfWork.Sale.PrepareCartItems(data);
            var paymentResult = _paymentService.MakePayment(data, user, cartItems);

            var result = new PaymentModel
            {
                TransactionType = TransactionType.Secure3d,
                ConversationId = paymentResult.ConversationId,
                HtmlContent = null,
                Locale = paymentResult.Locale,
                Status = paymentResult.Status
            };

            if (IsValidConversation(data.ConversationId, paymentResult))
                unitOfWork.Sale.Sell(data, cartItems, user.Id);
            else
                result.Error = new PaymentModelError
                {
                    ErrorCode = paymentResult.ErrorCode,
                    ErrorGroup = paymentResult.ErrorGroup,
                    ErrorMessage = paymentResult.ErrorMessage
                };

            return result;
        }
        private bool IsValidConversation(Guid determinedConversationId, Payment paymentResult)
        {
            if (paymentResult.Status == "success"
                && determinedConversationId.ToString() == paymentResult.ConversationId
                && paymentResult.ErrorCode == null
                && paymentResult.ErrorMessage == null
                && paymentResult.ErrorGroup == null)
                return true;
            return false;
        }
    }
}
