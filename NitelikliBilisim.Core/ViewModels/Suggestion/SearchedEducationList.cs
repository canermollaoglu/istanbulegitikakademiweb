using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Suggestion
{
    public class SearchedEducationList
    {
        public string Key { get; set; }
        public int SearchedCount { get; set; }
        public List<EducationDetail> EducationDetails { get; set; } = new List<EducationDetail>();
    }
}
