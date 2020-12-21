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
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryShortDescription { get; set; }
        public Dictionary<int, string> OrderTypes { get; set; }
        public Dictionary<int, string> EducationHostCities { get; set; }
    }
}
