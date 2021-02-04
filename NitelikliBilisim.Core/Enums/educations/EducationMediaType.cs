using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums
{
    public enum EducationMediaType
    {
        [Description("Kart Görseli (397X200)")]
        Card = 1010,
        [Description("Liste Görseli (150X95)")]
        List = 1020,
        [Description("Detay Sayfası Görseli (450X220)")]
        Detail = 1050,
        [Description("Kare Görsel (200X200)")]
        Square = 1060
    }
}