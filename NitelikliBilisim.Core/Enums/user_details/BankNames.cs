using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NitelikliBilisim.Core.Enums.user_details
{
    public enum BankNames
    {
        [Description("Türkiye Cumhuriyeti Ziraat Bankası A.Ş.")]
        ZiraatBank = 1000,
        [Description("Türkiye İş Bankası A.Ş.")]
        IsBank = 1010,
        [Description("Türkiye Garanti Bankası A.Ş.")]
        Garanti = 1020,
        [Description("Yapı ve Kredi Bankası A.Ş.")]
        YapiKredi = 1030,
        [Description("Türkiye Halk Bankası A.Ş.")]
        HalkBank = 1040,
        [Description("Türkiye Vakıflar Bankası T.A.O.")]
        VakifBank = 1050,
        [Description("Akbank T.A.Ş.")]
        AkBank = 1060,
        [Description("Denizbank A.Ş.")]
        DenizBank = 1070,
        [Description("Finansbank A.Ş.")]
        FinansBank = 1080,
        [Description("Türk Ekonomi Bankası A.Ş.")]
        TEB = 1090
    }
}
