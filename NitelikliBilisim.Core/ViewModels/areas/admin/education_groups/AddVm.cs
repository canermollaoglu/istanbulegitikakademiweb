using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class AddGetVm
    {
        public List<Education> Educations { get; set; }
        public List<EducationHost> Hosts { get; set; }
    }

    public class _Education
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
