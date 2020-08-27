using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class CancellationFormPostVm
    {
        public Guid TicketId { get; set; }
        public Guid InvoiceId { get; set; }
        /// <summary>
        /// Tekil İptallerde kullanılmak üzere oluşturulmuştur.
        /// </summary>
        public Guid? InvoiceDetailId { get; set; }
        public string UserDescription { get; set; }
    }
}
