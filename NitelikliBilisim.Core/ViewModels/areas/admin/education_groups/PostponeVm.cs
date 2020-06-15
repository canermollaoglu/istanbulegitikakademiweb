using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class EliminatedAndNewDates
    {
        public List<DateTime> EliminatedDates { get; set; }
        public List<DateTime> NewDates { get; set; }
        public List<string> EliminatedDateTexts { get; set; }
        public List<string> NewDateTexts { get; set; }
    }
}
