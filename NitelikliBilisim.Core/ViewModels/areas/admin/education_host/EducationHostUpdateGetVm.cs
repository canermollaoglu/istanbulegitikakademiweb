using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_host
{
    public class EducationHostUpdateGetVm
    {
        public EducationHost EducationHost{ get; set; }
        public Dictionary<int,string> HostCities { get; set; }
    }
}
