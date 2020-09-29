using System;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class StudentTicketsVm
    {
        public Guid TicketId { get; set; }
        public string EducationName { get; set; }
        public string HostName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsUsed { get; set; }
    }
}
