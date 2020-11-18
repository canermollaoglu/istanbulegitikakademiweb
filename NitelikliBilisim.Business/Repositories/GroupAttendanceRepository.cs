using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_group_attendances;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupAttendanceRepository : BaseRepository<GroupAttendance, Guid>
    {
        private readonly NbDataContext _context;
        public GroupAttendanceRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public AttendanceVm GetAttendances(Guid groupId, DateTime date)
        {
            var groupStudents = _context.Bridge_GroupStudents
                .Where(x => x.Id == groupId)
                .Join(_context.Users, l => l.Id2, r => r.Id, (x, y) => new
                {
                    GroupStudent = x,
                    Customer = y
                }).ToList();
            var attendances = _context.GroupAttendances
                .Where(x => x.GroupId == groupId && x.Date == date)
                .ToList();

            var model = new AttendanceVm
            {
                GroupId = groupId,
                Date = date
            };
            foreach (var item in groupStudents)
            {
                var attendance = attendances.FirstOrDefault(x => x.CustomerId == item.Customer.Id);
                model.Attendances.Add(new _Attendance
                {
                    CustomerId = item.Customer.Id,
                    Name = item.Customer.Name.ToUpper(),
                    Surname = item.Customer.Surname.ToUpper(),
                    IsAttended = attendance == null,
                    Reason = attendance != null ? attendance.Reason : ""
                });
            }
            return model;
        }

        public void SaveAttendances(AttendanceData data)
        {
            if (data.Date.Date > DateTime.Now.Date)
                throw new Exception("Yoklama kaydı sonraki günler için girilemez!");
            if (data.StudentRecords == null || data.StudentRecords.Count == 0)
                throw new Exception("Grupta öğrenci bulunmamaktadır!");

            var attendanceRecords = _context.GroupAttendances
                .Where(x => x.GroupId == data.GroupId && x.Date == data.Date)
                .ToList();

            var addedRecords = new List<GroupAttendance>();
            var removedRecords = new List<GroupAttendance>();
            var attendanceCustomerIds = attendanceRecords.Select(x => x.CustomerId);
            foreach (var item in data.StudentRecords)
            {
                if (!attendanceCustomerIds.Contains(item.CustomerId) && !item.IsAttended)
                {
                    addedRecords.Add(new GroupAttendance
                    {
                        CustomerId = item.CustomerId,
                        Date = data.Date,
                        GroupId = data.GroupId,
                        Reason = item.Reason
                    });
                }
                if (attendanceCustomerIds.Contains(item.CustomerId) && item.IsAttended)
                {
                    var attendance = attendanceRecords.First(x => x.CustomerId == item.CustomerId);
                    removedRecords.Add(attendance);
                }
                if (attendanceCustomerIds.Contains(item.CustomerId) && !item.IsAttended)
                {
                    var attendance = attendanceRecords.First(x => x.CustomerId == item.CustomerId);
                    attendance.Reason = item.Reason;
                }
            }
            var lessonDays = _context.GroupLessonDays
                .Include(x => x.Group)
                .ThenInclude(x => x.Education)
                .Where(x => x.GroupId == data.GroupId);
            var lessonDay = lessonDays.FirstOrDefault(x => x.DateOfLesson == data.Date);
            lessonDay.HasAttendanceRecord = true;
            _context.GroupAttendances.AddRange(addedRecords);
            _context.GroupAttendances.RemoveRange(removedRecords);
            _context.SaveChanges();
            if (lessonDay == null)
                return;
            var salary = _context.EducatorSalaries.FirstOrDefault(x => x.EducatorId == lessonDay.EducatorId && x.EarnedForGroup == lessonDay.GroupId);
            if (salary == null)
            {
                _context.EducatorSalaries.Add(new EducatorSalary
                {
                    EarnedAt = data.Date,
                    EducatorId = lessonDay.EducatorId,
                    EarnedForGroup = data.GroupId,
                    Paid = lessonDay.EducatorSalary.GetValueOrDefault(0) * lessonDay.Group.Education.HoursPerDay
                });
            }
            else
            {
                salary.Paid = (lessonDays.Count(x => x.HasAttendanceRecord)) * lessonDay.EducatorSalary.GetValueOrDefault() * lessonDay.Group.Education.HoursPerDay;
            }
            _context.SaveChanges();
        }
    }
}
