using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class FavoriteEducationVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string HoursText { get; set; }
        public string DaysText { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string SeoUrl { get; set; }
    }
}
