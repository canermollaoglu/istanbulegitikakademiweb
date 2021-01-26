using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class PaymentSuccessDetailVm
    {
        public string TotalDiscount { get; set; }
        public string TotalNewPrice { get; set; }
        public List<PaymentSuccessGroupDetailVm> InvoiceDetails { get; set; }
        public string TotalOldPrice { get; set; }
        public string Installment { get; set; }
    }
}
