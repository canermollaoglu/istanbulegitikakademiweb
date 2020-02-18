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
        public string GroupName { get; set; }
        public string Educator { get; set; }
        public string Host { get; set; }
    }
    public class _LessonDay
    {
        public string LessonDateText { get; set; }
    }
}
