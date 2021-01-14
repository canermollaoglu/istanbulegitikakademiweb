using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Wizard
{
    public class WizardSecondStepData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<WizardSecondStepSubData> SubCategories { get; set; } = new List<WizardSecondStepSubData>();
    }
}
