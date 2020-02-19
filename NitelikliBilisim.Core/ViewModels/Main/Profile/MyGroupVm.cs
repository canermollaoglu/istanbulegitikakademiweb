using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyGroupVm
    {
        public _Group Group { get; set; }
        public List<_LessonDay> LessonDays { get; set; }
    }

    public class _Group
    {
        public DateTime StartDate { get; set; }
        public string GroupName { get; set; }
        public string Educator { get; set; }
        public string Host { get; set; }
    }
    public class _LessonDay
    {
        public DateTime LessonDate { get; set; }
        public string LessonDateText { get; set; }
    }
}
