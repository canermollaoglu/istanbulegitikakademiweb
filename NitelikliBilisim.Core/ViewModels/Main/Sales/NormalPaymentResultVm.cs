using NitelikliBilisim.Core.PaymentModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class NormalPaymentResultVm
    {
        public string Message { get; set; }
        public PaymentResultStatus Status { get; set; }
        public PaymentSuccessDetailVm SuccessDetails { get; set; }
    }
}
