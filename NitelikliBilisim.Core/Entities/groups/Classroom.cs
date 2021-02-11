using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    public class Classroom : BaseEntity<Guid>
    {
        public Classroom()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(64)]
        public string Name { get; set; }

        [ForeignKey("Host")]
        public Guid HostId { get; set; }
        public virtual EducationHost Host { get; set; }
    }
}
