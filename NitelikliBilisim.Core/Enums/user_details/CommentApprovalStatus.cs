using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Enums.user_details
{
    public enum CommentApprovalStatus
    {
        [Description("Onay Bekliyor")]
        Waiting = 1000,
        [Description("Onaylandı")]
        Approved =1010,
        [Description("Reddedildi")]
        Refuse =1020
    }
}
