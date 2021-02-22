using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.dashboard
{
    public class AdminDashboardWidgetVm
    {
        /// <summary>
        /// Bu ay hesaplanan değer
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Bu ayın geçen aya oranı
        /// </summary>
        public string Rate { get; set; }
        /// <summary>
        /// Değişimin pozitiflik durumu
        /// </summary>
        public bool IsPositive { get; set; }
    }
}
