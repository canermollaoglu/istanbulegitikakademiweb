using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        public EmailRepository(NbDataContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public string GetUserEmail(string userId)
        {
            return _context.Users.First(x => x.Id == userId).Email;
        }
        public string[] GetAdminEmails()
        {
            return _configuration.GetSection("SiteGeneralOptions").GetSection("AdminEmails").Value.Split(";");
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

        public string GetEmailByEducatorId(string educatorId)
        {
            return _context.Users.First(x => x.Id == educatorId).Email;
        }
    }
}
