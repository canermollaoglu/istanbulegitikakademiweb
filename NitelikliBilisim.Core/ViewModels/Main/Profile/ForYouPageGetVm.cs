using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class ForYouPageGetVm
    {
        public int EducationWeek { get; set; }
        public int LeftWeeks { get; set; }
        public string NbuyStartDate { get; set; }
        public string NbuyEndDate { get; set; }
        public string EducationCategory { get; set; }
        public List<EducationMonth> EducationMonths { get; set; } = new List<EducationMonth>();

    }

    public class EducationMonth
    {
        public int Order { get; set; }
        public List<EducationWeek> Weeks { get; set; } = new List<EducationWeek>();
    }
    public class EducationWeek
    {
        public int Order { get; set; }
        public bool IsCurrentWeek { get; set; }
    }
}
