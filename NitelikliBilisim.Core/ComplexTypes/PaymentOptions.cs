using Iyzipay;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class PaymentOptions : Options
    {
        public const string Key = "IyzicoOptions";
        public string ThreedsCallbackUrl { get; set; }
    }
}