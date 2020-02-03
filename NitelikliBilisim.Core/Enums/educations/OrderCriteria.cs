using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NitelikliBilisim.Core.Enums
{
    public enum OrderCriteria
    {
        [Description("Son Eklenenler")]
        Latest = 1010,
        [Description("Seviye")]
        Popular = 1020
    }
}
