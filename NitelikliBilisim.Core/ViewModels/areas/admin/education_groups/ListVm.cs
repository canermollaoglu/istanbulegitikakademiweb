using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class _Group
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
        public string Location { get; set; }
        public string EducationName { get; set; }
        public int AssignedCount { get; set; }
        public byte Quota { get; set; }
        public string EducatorName { get; set; }
    }
}
