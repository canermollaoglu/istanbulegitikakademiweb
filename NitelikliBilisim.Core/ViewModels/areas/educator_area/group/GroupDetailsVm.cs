using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.educator_area.group
{
    public class GroupDetailsVm
    {

    }

    public class _EducationDay
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DateText { get; set; }
        public bool HasAttendanceRecord { get; set; }
    }
}
