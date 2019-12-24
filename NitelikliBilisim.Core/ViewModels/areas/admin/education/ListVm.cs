using NitelikliBilisim.Core.DTO;
using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education
{
    public class ListGetVm
    {
        public List<EducationDto> Educations { get; set; }
        public List<_EducationSub> Medias { get; set; }
        public List<_EducationSub> Parts { get; set; }
        public List<_EducationSub> Gains { get; set; }
        public List<_EducationCategory> EducationCategories { get; set; }
    }

    public class _EducationSub
    {
        public Guid EducationId { get; set; }
        public int Count { get; set; }
    }
    public class _EducationCategory
    {
        public Guid EducationId { get; set; }
        public string ConcattedCategories { get; set; }
    }
}
