using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class GeneralSalesReportVm
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string EducationName { get; set; }
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal MerchantPayout { get; set; }
        public DateTime BlockageResolveDate { get; set; }
    }
}
