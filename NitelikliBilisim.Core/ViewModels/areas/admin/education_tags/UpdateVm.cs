using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_tags
{
    public class UpdateGetVm : AddGetVm
    {
        public EducationTag Tag { get; set; }
        public EducationTag BaseTag { get; set; }
    }
    public class UpdatePostVm : AddPostVm
    {
        public Guid TagId { get; set; }
    }
}
