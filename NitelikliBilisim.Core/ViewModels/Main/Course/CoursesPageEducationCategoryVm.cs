using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class CoursesPageEducationCategoryVm
    {
        public Guid Id { get; set; }
        public string SeoUrl { get; set; }
        public string Name { get; set; }
        public int EducationCount { get; set; }
    }
}
