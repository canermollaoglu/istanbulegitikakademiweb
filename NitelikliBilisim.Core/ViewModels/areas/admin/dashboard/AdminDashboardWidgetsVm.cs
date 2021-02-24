using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.dashboard
{
    public class AdminDashboardWidgetsVm
    {
        public AdminDashboardWidgetVm StudentInfoWidget { get; set; }
        public AdminDashboardWidgetVm SalesInfoWidget { get; set; }
        public AdminDashboardWidgetVm EducationGroupWidget { get; set; }
        public AdminDashboardWidgetVm ProfitWidget { get; set; }
    }
}
