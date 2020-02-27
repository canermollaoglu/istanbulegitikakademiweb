using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_group_attendances
{
    public class AttendanceVm
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsAttended { get; set; }
        public string Reason { get; set; }
    }
}
