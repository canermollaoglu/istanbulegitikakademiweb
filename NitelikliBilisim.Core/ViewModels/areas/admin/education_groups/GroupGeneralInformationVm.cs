using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class GroupGeneralInformationVm
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string EducationHost { get; set; }
        public string Classroom { get; set; }
        public string EducatorName { get; set; }
        public string StartDate { get; set; }
        public int Quota { get; set; }
        public int AssignedStudentsCount { get; set; }
        public string EducationName { get; set; }
    }
}
