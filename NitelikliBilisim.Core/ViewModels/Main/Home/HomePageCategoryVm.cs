using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Home
{
    public class HomePageCategoryVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int EducationCount { get; set; }
        public string IconColor { get; set; }
        public string SeoUrl { get; set; }
    }
}
