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
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.Price = "1";
            request.PaidPrice = "1.2";
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
            return View();
        }
    }
}