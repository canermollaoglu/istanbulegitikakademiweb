using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_component
{
    public class EducationComponentItemListVm
    {
        public Guid Id { get; set; }
        public string EducationName { get; set; }
        public string Category { get; set; }
        public string BaseCategory { get; set; }
        public int Order { get; set; }
        public EducationComponentSuggestionType SuggestionType { get; set; }
        public EducationComponentType ComponentType { get; set; }
    }
}
