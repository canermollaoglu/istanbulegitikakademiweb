using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion
{
    public class EducationSuggestionCriterionAddVm
    {
        public Guid EducationId { get; set; }
        [Required(ErrorMessage ="Kriter tipi seçimi zorunludur.")]
        public int CriterionType { get; set; }
        [Required(ErrorMessage ="Kriter için minimum değer girilmeden geçilemez.")]
        public int MinValue { get; set; }
        public int? MaxValue { get; set; }

    }
}
