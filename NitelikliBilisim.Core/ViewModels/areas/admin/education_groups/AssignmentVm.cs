using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class AssignPostVm
    {
        public Guid GroupId { get; set; }
        public Guid TicketId { get; set; }
    }

    public class UnassignPostVm
    {
        public Guid TicketId { get; set; }
    }
}
