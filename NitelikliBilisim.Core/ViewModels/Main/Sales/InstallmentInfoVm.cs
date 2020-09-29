using NitelikliBilisim.Core.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Sales
{
    public class InstallmentInfoVm
    {
        public Guid ConversationId { get; set; } = Guid.NewGuid();
        public string CardNumber { get; set; }
        public List<_CartItem> CartItems { get; set; }
    }
}
