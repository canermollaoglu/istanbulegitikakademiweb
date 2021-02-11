using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_host
{
    /// <summary>
    /// HostCity'den farklı alanlar açılabilmesi durumuna karşın vm oluşturuldu
    /// </summary>
    public class EducationHostAddGetVm
    {
        public Dictionary<int, string> HostCities { get; set; }
    }
}
