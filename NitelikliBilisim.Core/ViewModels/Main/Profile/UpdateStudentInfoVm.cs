using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class UpdateStudentInfoVm
    {
        public string UserId { get; set; }
        public Genders Gender { get; set; }
        public Jobs Job { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? LastGraduatedSchoolId { get; set; }

    }
}
