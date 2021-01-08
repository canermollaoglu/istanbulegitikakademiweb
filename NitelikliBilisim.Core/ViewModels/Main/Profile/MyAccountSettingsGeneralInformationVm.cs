using NitelikliBilisim.Core.Enums.user_details;
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
        public Jobs Job { get; set; }
        public int? LastGraduatedSchoolId { get; set; }
        public string Phone { get; set; }
        public string LinkedIn { get; set; }
        public string WebSite { get; set; }
        public string AvatarPath { get; set; }
        public int? CityId { get; set; }
        public Genders Gender { get; set; }
        public NbuyInformationVm NbuyInformation { get; set; }
        public bool IsNbuyStudent { get; set; }
    }
}
