using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Models.Education
{
    public class UpdateGetVm : AddGetVm
    {
        public Core.Entities.Education Education { get; set; }
        public List<EducationCategory> RelatedCategories { get; set; }
    }

    public class UpdatePostVm : EducationCrudVm
    {
        public Guid EducationId { get; set; }
    }
}
