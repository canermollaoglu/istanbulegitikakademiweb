using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class CalculateExpectedProfitabilityReturnVm
    {
        public int MinStudentCount { get; set; }
        public decimal PlannedAmount { get; set; }
        public int ExpectedRateOfProfitability { get; set; }
    }
}
