using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.educator_area.group
{
    public class MyGroupsGetVm
    {
        public List<_Group> Groups { get; set; }
    }
    public class _Group
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string EducationName { get; set; }
    }
}
