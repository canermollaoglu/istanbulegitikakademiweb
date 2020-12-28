using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyAccountSettingsGeneralInformationVm
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Job { get; set; }
        public int? LastGraduatedSchoolId { get; set; }
        public string Phone { get; set; }
        public string LinkedIn { get; set; }
        public string WebSite { get; set; }
    }
}
