using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EmailRepository
    {
        private readonly NbDataContext _context;
        public EmailRepository(NbDataContext context)
        {
            _context = context;
        }
        public List<string> GetAdminEmails()
        {
            return _context.UserRoles
                .Include(x => x.Role)
                .Include(x => x.User)
                .Where(x => x.Role.Name == "Admin")
                .Select(x => x.User.Email)
                .ToList();
        }
        public List<string> GetEmailsOfTeachersByGroup(Guid groupId)
        {
            var educators = _context.GroupLessonDays
                .Where(x => x.GroupId == groupId)
                .Select(x => x.EducatorId);
            return _context.Users
                .Where(x => educators.Contains(x.Id))
                .Select(x => x.Email)
                .ToList();
        }
        public string GetEmailOfTeacherAtDate(Guid groupId, DateTime at)
        {
            var educatorId = _context.GroupLessonDays.First(x => x.GroupId == groupId && x.DateOfLesson == at).EducatorId;
            return _context.Users.First(x => x.Id == educatorId).Email;
        }
        public List<string> GetEmailsOfStudentsByGroup(Guid groupId)
        {
            return _context.Bridge_GroupStudents
                .Where(x => x.Id == groupId)
                .Include(x => x.Customer)
                .ThenInclude(x => x.User)
                .Select(x => x.Customer.User.Email)
                .ToList();
        }
    }
}
