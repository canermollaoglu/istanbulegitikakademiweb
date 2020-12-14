using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyCouponVm
    {
        public Guid PromotionId { get;  set; }
        public string PromotionName { get;  set; }
        public string Code { get;  set; }
        public string RemainingTime { get;  set; }
        public PromotionStatus Status { get; set; }
    }
}
