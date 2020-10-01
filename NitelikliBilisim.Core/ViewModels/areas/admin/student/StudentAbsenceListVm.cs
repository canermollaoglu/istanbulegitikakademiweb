using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class StudentAbsenceListVm
    {
        public Guid Id { get; set; }
        public string EducationName { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public string GroupName { get; set; }
    }
}
