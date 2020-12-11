using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyCourseVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Days { get; set; }
        public string Hours { get; set; }
        public string FeaturedImageUrl { get; set; }
        public Guid EducationId { get; set; }
        public bool IsFavorite { get; set; }
    }
}
