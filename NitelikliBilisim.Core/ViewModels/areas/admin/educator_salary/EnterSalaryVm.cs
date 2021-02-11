using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator_salary
{
    public class EnterSalaryVm
    {
        public List<_Salary> Salaries { get; set; }
    }

    public class _Salary
    {
        public decimal Paid { get; set; }
        public string EducatorId { get; set; }
        public DateTime EarnedAt { get; set; }
        public Guid? EarnedForGroup { get; set; }
        public string GroupName { get; set; }
        public string EducatorName { get; set; }
    }
}
