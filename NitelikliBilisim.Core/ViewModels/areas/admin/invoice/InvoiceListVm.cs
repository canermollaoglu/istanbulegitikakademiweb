using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.invoice
{
    public class InvoiceListVm
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public decimal PaidPrice { get; set; }
        public string PaymentId { get; set; }
        public string InvoicePdfUrl { get; set; }
    }
}
