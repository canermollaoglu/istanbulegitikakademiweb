using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.AboutUs
{
    public class AboutUsGetVm
    {
        public List<EducationHostVm> Hosts { get; set; }
        public int TotalEducatorCount { get; set; }
        public int TotalEducationHours { get; set; }
        public List<EducatorListVm> Educators { get; set; }
    }
}
