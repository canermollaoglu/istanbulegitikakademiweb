using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_parts
{
    public class UpdateGetVm : AddPartVm
    {
        public Guid Id { get; set; }
        public List<_EducationPart> BaseParts { get; set; }
    }
    public class UpdatePostVm : AddPartVm
    {
        public Guid PartId { get; set; }
    }
}
