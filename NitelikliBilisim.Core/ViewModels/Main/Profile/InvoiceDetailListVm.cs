using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class InvoiceDetailListVm
    {
        public Guid Id { get; set; }
        public string PaidPrice { get; set; }
        public string Education { get; set; }
        public string EducationImage { get; set; }
        public string CategorySeoUrl { get; set; }
        public string EducationSeoUrl { get; set; }
    }
}
