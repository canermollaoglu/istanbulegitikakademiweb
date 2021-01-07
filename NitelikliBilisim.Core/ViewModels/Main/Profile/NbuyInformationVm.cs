using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class NbuyInformationVm
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime StartAt { get; set; }
        public EducationCenter EducationCenter { get; set; }
    }
}
