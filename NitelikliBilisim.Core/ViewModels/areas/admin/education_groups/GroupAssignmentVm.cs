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
        public List<Ticket> EligibleTickets { get; set; }
        public List<Bridge_GroupStudent> AssignedStudents { get; set; }
    }

}
