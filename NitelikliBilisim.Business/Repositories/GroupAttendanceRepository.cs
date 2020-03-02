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
            _context.GroupAttendances.AddRange(addedRecords);
            _context.GroupAttendances.RemoveRange(removedRecords);
            _context.SaveChanges();
        }
    }
}
