using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class SearchResultsVm
    {
        public List<CoursesPageEducationCategoryVm> Categories { get; set; }
        public Dictionary<int, string> OrderTypes { get; set; }
        public Dictionary<int, string> EducationHostCities { get; set; }
        public int TotalEducationCount { get; set; }
        public string SearchKey { get; set; }
        public int? HostCity { get; set; }
    }
}
