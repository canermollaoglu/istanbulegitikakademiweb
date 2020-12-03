using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Educator
{
    public class GetEducatorDetailVm
    {
        public GetEducatorDetailItemVm EducatorDetail { get; set; }
        public List<SuggestedEducationVm> PopularEducations { get; set; }
    }
}
