using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Enums
{
    public enum EducationComponentType
    {
        [Description("Popüler Proje Eğitimleri")]
        PopularProjectEducations = 1000,
        [Description("Yeni Başlayan Proje Eğitimleri")]
        BeginnerProjectEducations = 1010,
    }

    public enum EducationComponentSuggestionType
    {
        [Description("Üyeler")]
        Customer = 1000,
        [Description("Misafir Kullanıcılar")]
        Guest = 1010,
    }
}
