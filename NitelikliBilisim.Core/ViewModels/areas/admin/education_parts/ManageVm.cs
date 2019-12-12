using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_parts
{
    public class ManageVm
    {
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
    }

    public class GetEducationPartsVm
    {
        public List<_EducationPart> EducationParts { get; set; }
    }

    public class _EducationPart
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public string Title { get; set; }
        public byte Order { get; set; }
        public byte Duration { get; set; }
    }
}
