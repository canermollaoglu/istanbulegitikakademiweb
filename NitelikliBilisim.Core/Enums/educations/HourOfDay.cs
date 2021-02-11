using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NitelikliBilisim.Core.Enums
{
    public enum HourOfDay
    {
        [Description("08:00")]_0800, [Description("08:30")]_0830, [Description("09:00")]_0900,
        [Description("09:30")]_0930, [Description("10:00")]_1000, [Description("10:30")]_1030,
        [Description("11:00")]_1100, [Description("11:30")]_1130, [Description("12:00")]_1200,
        [Description("12:30")]_1230, [Description("13:00")]_1300, [Description("13:30")]_1330,
        [Description("14:00")]_1400, [Description("14:30")]_1430, [Description("15:00")]_1500,
        [Description("15:30")]_1530, [Description("16:00")]_1600, [Description("16:30")]_1630,
        [Description("17:00")]_1700, [Description("17:30")]_1730, [Description("18:00")]_1800,
        [Description("18:30")]_1830, [Description("19:00")]_1900, [Description("19:30")]_1930,
        [Description("20:00")]_2000
    }
}

//_0000, _0030, _0100, _0130, _0200,
//_0230, _0300, _0330, _0400, _0430,
//_0500, _0530, _0600, _0630, _0700,
//_0730, _2030, _2100, _2130, _2200,
//_2230, _2300, _2330