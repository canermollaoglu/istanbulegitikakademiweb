using Iyzipay;
using Microsoft.Extensions.Configuration;

namespace NitelikliBilisim.Core.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly Options _option;
        public PaymentService(IConfiguration configuration)
        {
            _option = configuration.GetSection("IyzicoOptions").Get<Options>();
        }

        public void MakePayment()
        {

        }

    }
}
