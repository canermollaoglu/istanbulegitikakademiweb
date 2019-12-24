using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.DTO
{
    public class EducationDto : BaseDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? NewPrice { get; set; }
        public byte Days { get; set; }
        public byte HoursPerDay { get; set; }
        public string Level { get; set; }
        public bool IsActive { get; set; }
    }
}
