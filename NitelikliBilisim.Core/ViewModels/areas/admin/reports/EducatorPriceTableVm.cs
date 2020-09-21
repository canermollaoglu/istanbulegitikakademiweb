using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class EducatorPriceTableVm
    {
        public string EducatorName { get; set; }
        public decimal AvgPrice { get; set; }
        public int EducationHour { get; set; }
        public decimal Sum { get; set; }
    }
}
