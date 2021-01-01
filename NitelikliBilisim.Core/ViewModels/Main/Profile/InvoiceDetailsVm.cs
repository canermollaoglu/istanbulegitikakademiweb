using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class InvoiceDetailsVm
    {
        public string PaymentId { get; set; }
        public DateTime Date { get; set; }
        public List<InvoiceDetailListVm> Details { get; set; }
        public string PaidPrice { get; set; }
        public byte InstallmentInfo { get; set; }
        public string TotalPrice { get; set; }
        public string DiscountAmount { get; set; }
    }
}
