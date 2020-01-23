using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class CourseDetailsVm
    {
        public EducationVm Details { get; set; }
        public List<EducatorVm> Educators { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
