using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main
{
    public class HeaderEducationMenuVm
    {
        public List<HeaderBaseCategory> BaseCategories { get; set; } = new List<HeaderBaseCategory>();
    }

    public class HeaderBaseCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string TotalEducationCount { get; set; }
        public string SeoUrl { get; set; }
        public List<HeaderSubCategory> SubCategories { get; set; } = new List<HeaderSubCategory>();
        public string IconColor { get; set; }
    }

    public class HeaderSubCategory
    {
        public Guid Id { get; set; }
        public string SeoUrl { get; set; }
        public string Name { get; set; }
        public List<HeaderEducation> Educations { get; set; } = new List<HeaderEducation>();
    }
    public class HeaderEducation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SeoUrl { get; set; }

    }

}
