using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Wizard
{
    public class WizardSuggestedEducationVm
    {
        public Guid Id { get; set; }
        public string SeoUrl { get; set; }
        public string CatSeoUrl { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public byte Day { get; set; }
        public int Hours { get; set; }
    }
}
