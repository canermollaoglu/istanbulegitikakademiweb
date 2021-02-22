using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class RefundVm
    {
        public Guid InvoiceDetailId { get; set; }
        [Required]
        public string UserDescription { get; set; }
    }
}
