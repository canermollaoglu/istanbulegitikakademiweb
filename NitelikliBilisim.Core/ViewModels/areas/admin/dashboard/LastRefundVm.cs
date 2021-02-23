using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.dashboard
{
    public class LastRefundVm
    {
        public Guid Id { get; set; }
        public DateTime RefundDate { get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
        public string EducationName { get; set; }
        public string GroupName { get; set; }
        public decimal RefundPrice { get; set; }
        public string StudentId { get; set; }
        public Guid GroupId { get; set; }
    }
}
