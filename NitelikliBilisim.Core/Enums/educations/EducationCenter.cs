using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NitelikliBilisim.Core.Enums
{
    public enum EducationCenter
    {
        [Description("Wissen Akademie")]
        Wissen = 1010,
        [Description("Ayvansaray Üniversitesi")]
        Ayvansaray = 1020,
        [Description("Bilge Adam")]
        BilgeAdam = 1030,
        [Description("Bilişim Eğitim Merkezi")]
        BilisimEgitimMerkezi = 1040,
        [Description("SmartPro Bilgisayar Akademisi")]
        SmartPro = 1050
    }
}
