using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.subscriber
{
    public class SubscriberPostVm
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public SubscriptionBroadcastType Type { get; set; }

    }
}
