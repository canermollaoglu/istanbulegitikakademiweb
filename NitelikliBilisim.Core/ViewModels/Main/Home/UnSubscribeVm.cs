using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Home
{
    public class UnSubscribeVm
    {
        public bool IsCancelled { get; set; }
        public SubscriberType SubscriberType { get; set; }
        public string SubscriberTypeText { get; set; }
    }
}
