using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class EducatorPriceTableVm
    {
        public string EducatorName { get; set; }
        public decimal AvgPrice { get; set; }
        public int TotalHours { get; set; }
        public decimal SumPrice { get; set; }
    }
}
