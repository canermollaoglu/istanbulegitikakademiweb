using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyAccountSettingsGetVm
    {
        public MyAccountSettingsGeneralInformationVm GeneralInformation { get; set; }
        public List<University> Universities { get; set; }
        public List<Address> Addresses { get; set; }
        public List<City> Cities { get; set; }
        public Dictionary<int, string> Genders { get; set; }
        public Dictionary<int, string> Jobs { get; set; }
        public Dictionary<int, string> EducationCenters { get; set; }
        public Dictionary<Guid, string> EducationCategories { get; set; }
        public int DefaultAddressId { get; set; }
    }
}
