using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion
{
    public class EducationSuggestionCriterionUpdateVM
    {
        public Guid Id { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public string[] CharValue { get; set; }

    }
}
