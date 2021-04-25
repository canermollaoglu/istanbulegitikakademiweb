using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.reports
{
    public class GroupFollowUpReportVm
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
        public string GroupName { get; set; }
        public DateTime GroupStartDate { get; set; }
        public byte EducationDay { get; set; }
        public DateTime GroupEndDate { get; set; }
        public int LeftDay { get; set; }
        public bool IsReserve { get; set; }
    }
}
