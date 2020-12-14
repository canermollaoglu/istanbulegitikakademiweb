using NitelikliBilisim.Core.ViewModels;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Models
{
    public class HomeIndexModel
    {
        public List<SuggestedEducationVm> SuggestedEducations { get; set; }
        public Dictionary<string, int> EducationCountByCategory { get; set; }
    }
}
