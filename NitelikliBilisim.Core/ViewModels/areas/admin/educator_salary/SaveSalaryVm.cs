using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator_salary
{
    public class SaveSalaryPostData
    {
        public DateTime Date { get; set; }
        public List<_SalaryData> Salaries { get; set; }
    }
    public class _SalaryData
    {
        public string educatorId { get; set; }
        public decimal paid { get; set; }
        public Guid groupId { get; set; }
    }
}
