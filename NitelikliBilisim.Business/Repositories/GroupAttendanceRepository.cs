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

        public List<AttendanceVm> GetAttendances(Guid groupId, DateTime date)
        {
            var groupStudents = _context.Bridge_GroupStudents
                .Where(x => x.Id == groupId)
                .Join(_context.Users, l => l.Id2, r => r.Id, (x, y) => new
                {
                    GroupStudents = x,
                    Customers = y
                }).ToList();
            var attendances = _context.GroupAttendances
                .Where(x => x.GroupId == groupId && x.Date == date)
                .ToList();

            var model = new List<AttendanceVm>();
            foreach (var item in groupStudents)
            {

            }
            return null;
        }
    }
}
