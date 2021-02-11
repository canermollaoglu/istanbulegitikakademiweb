using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums.user_details
{
    public enum Genders
    {
        [Description("Erkek")]
        Male = 1010,
        [Description("Kadın")]
        Female= 1020,
        [Description("Belirtmek İstemiyorum")]
        Undefined = 1030
    }
}
