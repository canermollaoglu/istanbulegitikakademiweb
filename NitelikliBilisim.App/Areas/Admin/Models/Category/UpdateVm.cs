using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.App.Areas.Admin.Models.Category
{
    public class UpdateGetVm : AddGetVm
    {
        public EducationCategory Category { get; set; }
        public EducationCategory BaseCategory { get; set; }
    }
    public class UpdatePostVm : AddPostVm
    {
        public Guid CategoryId { get; set; }
    }
}
