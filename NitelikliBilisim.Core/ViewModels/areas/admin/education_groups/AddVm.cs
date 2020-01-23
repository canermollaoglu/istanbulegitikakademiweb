using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public Guid? EducationId { get; set; }
        [Required]
        public string EducatorId { get; set; }
        [Required]
        public Guid? HostId { get; set; }
        [Required]
        public byte? Quota { get; set; }
        public List<int> LessonDays { get; set; }
    }

    public class _Education
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
