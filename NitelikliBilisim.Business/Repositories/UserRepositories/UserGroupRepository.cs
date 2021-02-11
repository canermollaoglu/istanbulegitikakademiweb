using Microsoft.EntityFrameworkCore;
using MUsefulMethods;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class UserGroupRepository
    {
        private readonly NbDataContext _context;
        public UserGroupRepository(NbDataContext context)
        {
            _context = context;
        }

        public MyGroupVm GetMyGroupVm(Guid ticketId)
        {
            var ticket = _context.Tickets
                .Include(x => x.Host)
                .FirstOrDefault(x => x.Id == ticketId);

            if (ticket == null)
                return null;

            var bridge = _context.Bridge_GroupStudents
                .Include(x => x.Group)
                .ThenInclude(x => x.Host)
                .FirstOrDefault(x => x.TicketId == ticket.Id);

            if (bridge == null)
                return null;

            var educator = _context.Educators
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == bridge.Group.EducatorId);
            var lessonDays = _context.GroupLessonDays
                .Where(x => x.GroupId == bridge.Group.Id)
                .ToList();

            return new MyGroupVm
            {
                Group = new _Group
                {
                    StartDate = bridge.Group.StartDate,
                    GroupName = bridge.Group.GroupName,
                    Educator = educator != null ? $"{educator.User.Name.ToUpper()} {educator.User.Surname.ToUpper()}" : "?",
                    Host = $"{bridge.Group.Host.HostName} ({EnumHelpers.GetDescription(bridge.Group.Host.City)})"
                },
                LessonDays = lessonDays.Select(x => new _LessonDay
                {
                    LessonDate = x.DateOfLesson,
                    LessonDateText = x.DateOfLesson.ToLongDateString()
                }).ToList()
            };
        }
    }
}
