using System;

namespace NitelikliBilisim.Core.DTO
{
    public class EducationDto : BaseDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Days { get; set; }
        public byte HoursPerDay { get; set; }
        public string Level { get; set; }
        public bool IsActive { get; set; }
        public int MediaCount { get; set; }
        public int PartCount { get; set; }
        public int GainCount { get; set; }
        public int EducatorCount { get; set; }
        public string EducationCategories { get; set; }
    }
}