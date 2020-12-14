using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class JoinedGroupVm
    {
        public string GroupName { get; set; }
        public string HostName { get; set; }
        public string EducationName { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime GroupStartDate { get; set; }
        public Guid GroupId { get; set; }
    }
}
