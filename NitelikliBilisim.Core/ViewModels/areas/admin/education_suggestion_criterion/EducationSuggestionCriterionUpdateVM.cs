using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion
{
    public class EducationSuggestionCriterionUpdateVM
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Değer girilmeden geçilemez.")]
        public int MinValue { get; set; }
        public int? MaxValue { get; set; }

    }
}
