using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator_salary;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var days = _context.GroupLessonDays
                .Where(x => x.DateOfLesson.Date == date.Date)
                .ToList();
            var salaries = _context.EducatorSalaries
                .Where(x => x.EarnedAt.Date == date.Date)
                .ToList();
            return model;
        }
    }
}
