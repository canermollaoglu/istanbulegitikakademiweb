using Iyzipay;
using Iyzipay.Model;
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
            
            return View();
        }
    }
}