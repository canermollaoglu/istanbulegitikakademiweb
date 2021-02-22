using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.dashboard
{
    public class AdminDashboardSalesChartDataVm
    {
        public ApexChartModel[] Values { get; set; }
    }
    public class ApexChartModel
    {
        public string x { get; set; }
        public string y { get; set; }
    }
}
