using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class EducationGroupWithInvoiceInfoVm
    {
        public Guid GroupId { get; set; }
        public decimal PaidPrice { get; set; }
        public DateTime StartDate { get; set; }
        public string EducationName { get; set; }
        public string GroupName { get; set; }
    }
}
