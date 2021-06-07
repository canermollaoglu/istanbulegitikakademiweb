using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Main.AboutUs;
using NitelikliBilisim.Core.ViewModels.Main.Course;
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
        public List<EducatorListVm> Educators { get; set; }
        public List<CoursesPageEducationCategoryVm> Categories { get; set; }
        public Dictionary<int, string> OrderTypes { get; set; }
        public Dictionary<int, string> EducationHostCities { get; set; }
        public int TotalEducationCount { get; set; }
        public string CategoryName { get; set; }
        public string CategoryShortDescription { get; set; }
        public Guid? CategoryId { get; set; }
        public FeaturedEducationVm FeaturedEducation { get; set; }
        public List<PopularTopic> PopularTopics { get; set; }
        public Guid EducationCategoryId { get; set; }
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
