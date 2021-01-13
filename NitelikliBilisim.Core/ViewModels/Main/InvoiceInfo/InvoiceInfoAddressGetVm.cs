using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo
{
    public class InvoiceInfoAddressGetVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsDefault { get; set; }
        public AddressTypes AddressType { get; set; }
    }
}
