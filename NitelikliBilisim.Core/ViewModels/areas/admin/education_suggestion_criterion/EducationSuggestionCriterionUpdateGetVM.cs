using NitelikliBilisim.Core.Enums.educations;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion
{
    public class EducationSuggestionCriterionUpdateGetVM 
    {
        public Guid Id { get; set; }
        public CriterionType CriterionType { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string CharValue { get; set; }
        public Dictionary<string,string> SelectedEducations { get; set; }
        public Dictionary<string,string> AllEducations { get; set; }
    }
}
