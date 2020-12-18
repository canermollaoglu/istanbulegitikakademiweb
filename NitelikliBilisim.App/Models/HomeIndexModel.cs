using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Models
{
    public class HomeIndexModel
    {
        public List<SuggestedEducationVm> SuggestedEducations { get; set; }
        public Dictionary<string, int> EducationCountByCategory { get; set; }
        public List<HighlightCommentVm> EducationComments { get; internal set; }
    }
}
