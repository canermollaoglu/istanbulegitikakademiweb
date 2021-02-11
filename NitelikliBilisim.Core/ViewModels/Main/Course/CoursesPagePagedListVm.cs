using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class CoursesPagePagedListVm
    {
        public List<CoursesPageEducationsVm> Educations { get; set; }
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int TotalPageCount { get; set; }
    }
}
