using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator
{
    public class ManageAssignEducatorsGetVm
    {
        public string EducationName { get; set; }
        public Guid EducationId { get; set; }
        public List<Educator> Educators { get; set; }
    }

    public class ManageAssignEducatorsPostVm
    {
        public Guid EducationId { get; set; }
        public List<string> Educators { get; set; }
    }
}
