using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums
{
    public enum CustomerType
    {
        [Description("Bireysel")]
        Individual = 1000,
        [Description("Kurumsal")]
        Corporate = 2000
    }
}
