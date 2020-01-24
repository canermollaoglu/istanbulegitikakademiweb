using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Bridge_EducationEducators")]
    public class Bridge_EducationEducator : BaseEntity2<Guid, string>
    {
        [ForeignKey("Id")]
        public virtual Education Education { get; set; }
        [ForeignKey("Id2")]
        public virtual Educator Educator { get; set; }
    }
}
