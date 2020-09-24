using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class GroupBasedCancellationSalesReport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? CancellationDate { get; set; }
        public decimal ListPrice { get; set; }
        public decimal PaidPrice { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal Commission { get; set; }
        public decimal MerchantPayout { get; set; }
        public decimal RefundPrice { get; set; }
    }
}
