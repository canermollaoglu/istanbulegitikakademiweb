using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator_salary;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorSalaryRepository : BaseRepository<EducatorSalary, Guid>
    {
        private readonly NbDataContext _context;
        public EducatorSalaryRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public EnterSalaryVm GetSalaries(DateTime date)
        {
            var model = new EnterSalaryVm
            {
                Salaries = new List<_Salary>()
            };

            return model;
        }
    }
}
