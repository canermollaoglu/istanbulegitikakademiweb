using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class TicketRepository : BaseRepository<Ticket, Guid>
    {
        private readonly NbDataContext _context;
        public TicketRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public void AssignTicket(AssignPostVm data)
        {
            // join?
            var ticket = _context.Tickets
                .FirstOrDefault(x => x.Id == data.TicketId);
            if (ticket == null)
                return;
            var group = _context.EducationGroups.FirstOrDefault(x => x.Id == data.GroupId);
            if (group == null)
                return;

            if (group.IsGroupOpenForAssignment
                && !_context.Bridge_GroupStudents.Any(x => x.Id == data.GroupId && x.Id2 == ticket.OwnerId))
            {
                _context.Bridge_GroupStudents.Add(new Bridge_GroupStudent
                {
                    Id = data.GroupId,
                    Id2 = ticket.OwnerId,
                    TicketId = data.TicketId
                });

                var groupStudentsCount = _context.Bridge_GroupStudents.Count(x => x.Id == group.Id) + 1;
                ticket.IsUsed = true;
                if (groupStudentsCount == group.Quota)
                    group.IsGroupOpenForAssignment = false;
                _context.SaveChanges();
            }
        }
        public void UnassignTicket(UnassignPostVm data)
        {
            // join?
            var ticket = _context.Tickets
                .FirstOrDefault(x => x.Id == data.TicketId);
            if (ticket == null)
                return;
            var bridgeGroupStudent = _context.Bridge_GroupStudents.FirstOrDefault(x => x.TicketId == data.TicketId);
            if (bridgeGroupStudent == null)
                return;
            var group = _context.EducationGroups.FirstOrDefault(x => x.Id == bridgeGroupStudent.Id);
            if (group == null)
                return;
            _context.Bridge_GroupStudents.Remove(bridgeGroupStudent);
            ticket.IsUsed = false;
            group.IsGroupOpenForAssignment = true;
            _context.SaveChanges();
        }
    }
}
