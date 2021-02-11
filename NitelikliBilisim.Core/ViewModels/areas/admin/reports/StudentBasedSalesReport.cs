using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class StudentBasedSalesReport
    {
        public decimal PaidPrice { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal MerchantPayout { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string EducationName { get; set; }
    }
}
