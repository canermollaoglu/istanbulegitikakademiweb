using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Models.Education
{
    public class UpdateGetVm : AddGetVm
    {
        public Core.Entities.Education Education { get; set; }
        public List<EducationTag> RelatedTags { get; set; }
        public EducationUpdateInfoVm EducationUpdateInfo { get; internal set; }
    }

    public class UpdatePostVm : EducationCrudVm
    {
        public Guid EducationId { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeauredEducation { get; set; }
        public _PostedFileUpdate BannerFile { get; set; }
        public _PostedFileUpdate PreviewFile { get; set; }
    }
}
