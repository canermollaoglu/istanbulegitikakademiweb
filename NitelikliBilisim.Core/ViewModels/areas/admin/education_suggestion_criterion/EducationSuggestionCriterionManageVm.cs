using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums.educations;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion
{
    public class EducationSuggestionCriterionManageVm
    {
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
        public Dictionary<int,string> EducationSuggestionCriterionTypes { get; set; }


    }
}
