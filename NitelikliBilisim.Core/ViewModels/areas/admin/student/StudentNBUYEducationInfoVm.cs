using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class StudentNBUYEducationInfoVm
    {
        public int EducationDay { get; set; }
        public DateTime StartDate { get; set; }
        public string CategoryName { get; set; }
        public string EducationCenter { get; set; }
        public bool IsCompleted { get; set; }
    }
}
