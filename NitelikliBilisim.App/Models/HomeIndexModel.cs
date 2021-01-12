﻿using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using NitelikliBilisim.Core.ViewModels.Main.Home;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Models
{
    public class HomeIndexModel
    {
        public List<SuggestedEducationVm> SuggestedEducations { get; set; }
        public Dictionary<string, int> EducationCountByCategory { get; set; }
        public List<HighlightCommentVm> EducationComments { get; internal set; }
        public Dictionary<int, string> HostCities { get; internal set; }
        public List<EducationSearchTag> EducationSearchTags { get; internal set; }
    }
}
