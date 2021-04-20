using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion
{
    public class EducationSuggestionCriterionListVm
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string EducationName { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public bool IsActive { get; set; }
    }
}
