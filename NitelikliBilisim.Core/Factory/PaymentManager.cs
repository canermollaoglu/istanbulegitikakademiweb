using NitelikliBilisim.Core.Factory.Payers;

namespace NitelikliBilisim.Core.Factory
{
    public class PaymentManager
    {
        private readonly IPayer _payer;
        public PaymentManager(IPayer payer)
        {
            _payer = payer;
        }

        public PaymentModel Pay()
        {
            return _payer.Pay();
        }
    }
}
