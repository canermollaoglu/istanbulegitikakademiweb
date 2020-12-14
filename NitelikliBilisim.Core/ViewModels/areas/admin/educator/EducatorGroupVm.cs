using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator
{
    public class EducatorGroupVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string HostName { get; set; }
        public string EducationName { get; set; }
        public decimal EducatorSalary { get; set; }
    }
}
