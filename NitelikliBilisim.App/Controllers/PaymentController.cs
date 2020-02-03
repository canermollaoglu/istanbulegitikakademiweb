using System;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Core.ViewModels.Sales;

namespace NitelikliBilisim.App.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly Options _option;

        public PaymentController(IConfiguration configuration)
        {
            _option = configuration.GetSection("IyzicoOptions").Get<Options>();
        }

        [HttpPost]
        public IActionResult MakePayment(PayPostVm model)
        {
            var conversationId = Guid.NewGuid();
            CreatePaymentRequest request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = conversationId.ToString(),
                Price = "1",
                PaidPrice = "1.2",
                Currency = Currency.TRY.ToString(),
                Installment = 1,
                BasketId = "B67832",
                PaymentChannel = model.PaymentChannel.ToString(),
                PaymentGroup = model.PaymentGroup.ToString()
            };


            return View();
        }
    }
}