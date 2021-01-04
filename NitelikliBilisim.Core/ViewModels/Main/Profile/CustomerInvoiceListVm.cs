using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class CustomerInvoiceListVm
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string PaidPrice { get; set; }
        public string FileUrl { get; set; }
        public string PaymentId { get; set; }
    }
}
