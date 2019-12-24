using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Bridge_EducationTags")]
    public class Bridge_EducationTag : BaseEntity2<Guid, Guid>
    {
        [ForeignKey("Id")]
        public virtual EducationTag Tag { get; set; }
        [ForeignKey("Id2")]
        public virtual Education Education { get; set; }
    }
}
