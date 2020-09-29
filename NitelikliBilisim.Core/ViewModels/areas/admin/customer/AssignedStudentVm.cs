using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.customer
{
    public class AssignedStudentVm
    {
        public string CustomerId { get; set; }
        public Guid TicketId { get; set; }
        public string CustomerFullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int NonAttendance { get; set; } = 0;
        public string Job { get; set; }
        public bool IsNbuyStudent { get; set; }
    }
}
