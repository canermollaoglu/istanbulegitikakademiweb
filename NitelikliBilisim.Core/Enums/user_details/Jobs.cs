using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums.user_details
{
    public enum Jobs
    {
        [Description("Bilgisayar Mühendisi")]
        BilgisayarMuhendisi = 1010,
        [Description("Yazılım Mühendisi")]
        YazilimMuhendisi = 1020,
        [Description("Memur")]
        Memur = 1030,
        [Description("Sistem Uzmanı")]
        SistemUzmani = 1040,
        [Description("Arayüz Geliştirici")]
        ArayuzGelistirici = 1050,
        [Description("Diğer")]
        Diger = 1060
    }
}
