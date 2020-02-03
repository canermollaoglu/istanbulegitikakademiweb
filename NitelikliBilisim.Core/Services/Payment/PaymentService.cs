using Iyzipay;
using Iyzipay.Request;
using Microsoft.Extensions.Configuration;

namespace NitelikliBilisim.Core.Services.Payment
{
    public class PaymentService : IPaymentService<CreatePaymentRequest, Iyzipay.Model.Payment>
    {
        private readonly Options _option;
        public PaymentService(IConfiguration configuration)
        {
            _option = configuration.GetSection("IyzicoOptions").Get<Options>();
        }

        public Iyzipay.Model.Payment MakePayment(CreatePaymentRequest request)
        {
            //CreatePaymentRequest request = new CreatePaymentRequest();
            
            return null;
        }

    }
}
