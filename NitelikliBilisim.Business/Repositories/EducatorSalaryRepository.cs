using Microsoft.EntityFrameworkCore;
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
            var educators = _context.UserRoles
                .Include(x => x.User)
                .Include(x => x.Role)
                .Where(x => x.Role.Name == "Educator")
                .AsNoTracking()
                .ToList();
            var days = _context.GroupLessonDays
                .Where(x => x.DateOfLesson.Date == date.Date)
                .Include(x => x.Group)
                .AsNoTracking()
                .ToList();
            var salaries = _context.EducatorSalaries
                .Where(x => x.EarnedAt.Date == date.Date)
                .AsNoTracking()
                .ToList();

            foreach (var item in days)
            {
                var educator = educators.First(x => x.UserId == item.EducatorId);
                var salary = salaries.FirstOrDefault(x => x.EducatorId == item.EducatorId);
                model.Salaries.Add(new _Salary
                {
                    EducatorId = item.EducatorId,
                    EarnedAt = item.DateOfLesson,
                    EarnedForGroup = item.Group.Id,
                    GroupName = item.Group.GroupName,
                    EducatorName = $"{educator.User.Name} {educator.User.Surname}",
                    Paid = salary != null ? salary.Paid : 0
                });
            }

            return model;
        }
        public void SaveSalaries(SaveSalaryPostData data)
        {
            var salaries = _context.EducatorSalaries
                .Where(x => x.EarnedAt == data.Date)
                .ToList();

            var newSalaries = new List<EducatorSalary>();
            foreach (var item in data.Salaries)
            {
                var salary = salaries.FirstOrDefault(x => x.EducatorId == item.educatorId);
                if (salary == null)
                    newSalaries.Add(new EducatorSalary
                    {
                        EarnedAt = data.Date,
                        EducatorId = item.educatorId,
                        EarnedForGroup = item.groupId,
                        Paid = item.paid
                    });
                else
                    salary.Paid = item.paid;
            }

            _context.EducatorSalaries.AddRange(newSalaries);
            _context.SaveChanges();
        }
    }
}
