using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
