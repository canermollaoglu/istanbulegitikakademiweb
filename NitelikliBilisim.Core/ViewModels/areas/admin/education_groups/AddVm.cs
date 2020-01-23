using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class AddGetVm
    {
        public List<Education> Educations { get; set; }
        public List<EducationHost> Hosts { get; set; }
    }

    public class AddPostVm
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public Guid EducationId { get; set; }
        public Guid EducatorId { get; set; }
        public Guid HostId { get; set; }
        public List<int> LessonDays { get; set; }
    }

    public class _Education
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
