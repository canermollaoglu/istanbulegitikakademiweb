using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationHosts")]
    public class EducationHost : BaseEntity<Guid>
    {
        public EducationHost()
        {
            Id = Guid.NewGuid();
        }

        public HostCity City { get; set; }
        [MaxLength(2048)]
        public string Address { get; set; }
        [MaxLength(256)]
        public string HostName { get; set; }
        [MaxLength(128)]
        public string Latitude { get; set; }
        [MaxLength(128)]
        public string Longitude { get; set; }

        public virtual List<EducationHostImage> EducationHostImages { get; set; }
    }
}
