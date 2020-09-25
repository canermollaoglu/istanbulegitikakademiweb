using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class CalculateSalesPriceGetVm
    {
        /// <summary>
        /// Kar Oranı
        /// </summary>
        public int ExpectedProfitRate { get; set; }

        /// <summary>
        /// Toplam Gider
        /// </summary>
        public decimal TotalExpenses { get; set; }
    }
}
