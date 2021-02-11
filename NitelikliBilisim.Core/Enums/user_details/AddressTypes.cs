using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Enums.user_details
{
    public enum AddressTypes
    {
        [Description("Bireysel")]
        Individual =1010,
        [Description("Kurumsal")]
        Corporate = 1020
    }
}
