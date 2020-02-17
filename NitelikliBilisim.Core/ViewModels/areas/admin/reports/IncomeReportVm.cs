using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class IncomeReportVm
    {
        public List<_Payout> Payouts { get; set; }
        public decimal SumOfNegative { get; set; }
        public decimal SumOfPositive { get; set; }
    }

    public class _Payout
    {
        public decimal PayoutNumeric { get; set; }
        public string PayoutText { get; set; }
        public bool IsNegative { get; set; }
        public DateTime PayoutDate { get; set; }
        public string PayoutDateText { get; set; }
        public string Payer { get; set; }
    }
}
