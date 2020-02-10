using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class AssignStudentsVm
    {
        public _Group Group { get; set; }
    }

    public class GetEligibleAndAssignedStudentsVm
    {
        public List<_Ticket> EligibleTickets { get; set; }
        public List<_Ticket> AssignedStudents { get; set; }
    }

    public class _Ticket
    {
        public Guid TicketId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
    }
}
