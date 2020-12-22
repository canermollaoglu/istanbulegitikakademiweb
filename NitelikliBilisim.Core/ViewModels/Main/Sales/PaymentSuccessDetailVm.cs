using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class PaymentSuccessDetailVm
    {
        public decimal TotalDiscount { get; set; }
        public decimal TotalNewPrice { get; set; }
        public List<PaymentSuccessGroupDetailVm> InvoiceDetails { get; set; }
        public decimal TotalOldPrice { get; set; }
        public string Installment { get; set; }
    }
}
