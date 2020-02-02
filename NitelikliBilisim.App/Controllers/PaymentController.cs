using Iyzipay;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.App.Controllers.Base;

namespace NitelikliBilisim.App.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly Options _option;
        private readonly IConfiguration _configuration;
        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
            // _option = _configuration.Configure<MySettingsModel>(Configuration.GetSection("MySettings"));  
;
        }

        public void Test()
        {

        }
    }
}