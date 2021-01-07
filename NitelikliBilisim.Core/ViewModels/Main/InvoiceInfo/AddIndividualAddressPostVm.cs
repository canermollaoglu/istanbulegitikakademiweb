using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo
{
    public class AddIndividualAddressPostVm
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string NameSurname { get; set; }
        public string IdentityNumber { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string CustomerId { get; set; }
        public bool IsDefaultAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
