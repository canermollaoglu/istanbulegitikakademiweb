using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo
{
    public class UpdateIndividualAddressPostVm
    {
        public int Id { get; set; }
        public string UpdateITitle { get; set; }
        public string UpdateIContent { get; set; }
        public string UpdateINameSurname { get; set; }
        public int? UpdateICityId { get; set; }
        public int? UpdateIStateId { get; set; }
        public string UpdateIPhone { get; set; }
    }
}
