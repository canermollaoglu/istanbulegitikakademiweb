using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class EducationOfTheWeekVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategorySeoUrl { get; set; }
        public byte Day { get; set; }
        public int Hour { get; set; }
        public string SeoUrl { get; set; }
        public string Image { get; set; }
        public double AppropriateCriterionCount { get; set; }
        public string Price { get; set; }
        public string CardImage { get; set; }
        public string CategoryName { get; set; }
    }
}
