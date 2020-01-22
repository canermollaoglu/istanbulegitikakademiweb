using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class AddGetVm
    {
        public List<_Education> Educations { get; set; }
    }

    public class _Education
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
