using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo
{
    public class UpdateCorporateAddressPostVm
    {
        public int Id { get; set; }
        public string UpdateCTitle { get; set; }
        public string UpdateCContent { get; set; }
        public string UpdateCCompanyName { get; set; }
        public string UpdateCTaxOffice { get; set; }
        public string UpdateCTaxNumber { get; set; }
        public int? UpdateCCityId { get; set; }
        public int? UpdateCStateId { get; set; }
        public string UpdateCPhone { get; set; }
    }
}
