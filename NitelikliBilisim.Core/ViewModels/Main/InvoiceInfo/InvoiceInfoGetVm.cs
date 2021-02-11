using NitelikliBilisim.Core.Entities.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo
{
    public class InvoiceInfoGetVm
    {
        public List<InvoiceInfoAddressGetVm> Addresses { get; set; }
        public List<City> Cities { get; set; }
        public int DefaultAddressId { get; set; }
    }
}
