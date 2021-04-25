using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.Enums.suggestion;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.suggestion
{
    public class WeeklySuggestedEducationsVm
    {
        public string SeoUrl { get; set; }
        /// <summary>
        /// Önerilecek eğitimin kategorisinin öğrencinin kategorisi olup olmadığı bilgisidir. Öğrencinin kategorisinde olan eğitimler önceliklidir.
        /// </summary>
        public bool IsCurrentCategory { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public int Point { get; set; }
        public WeekType WeekType { get; set; }
    }
}
