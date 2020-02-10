using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Bridge_GroupStudents")]
    public class Bridge_GroupStudent : BaseEntity2<Guid, string>
    {
        [ForeignKey("Id")]
        public virtual EducationGroup Group { get; set; }
        [ForeignKey("Id2")]
        public virtual Customer Customer { get; set; }

    }
}
