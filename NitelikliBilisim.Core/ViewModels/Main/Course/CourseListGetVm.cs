using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class CourseListGetVm
    {
        public List<CoursesPageEducationCategoryVm> Categories { get; set; }
        public int TotalEducationCount { get; set; }
    }
}
