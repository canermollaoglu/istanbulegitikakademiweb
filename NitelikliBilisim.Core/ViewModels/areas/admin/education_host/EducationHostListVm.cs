using MUsefulMethods;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_host
{
    public class EducationHostListVm
    {
        public Guid Id { get; set; }
        public HostCity City { get; set; }
        public string Address { get; set; }
        public string HostName { get; set; }

    }
}
