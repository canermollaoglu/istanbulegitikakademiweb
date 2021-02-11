using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Suggestion
{
    public class EducationDetailLog
    {
        public List<SearchedEducationList> SearchedEducations { get; set; }
        public List<ViewingEducation> ViewingEducations { get; set; }
        public List<EducationPoint> EducationTotalPoint { get; set; }
        public int TotalEducationViewCount { get; set; }
        public int TotalEducationSearchCount { get; set; }
    }
}
