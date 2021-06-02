using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.Entities;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_component
{
    public class EducationComponentAddGetVm
    {
        public Dictionary<int, string> ComponentTypes { get; set; }
        public Dictionary<int, string> SuggestionTypes { get; set; }
        public List<Education> Educations { get; set; }
    }
}
