using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Bridge_EducationCategories")]
    public class Bridge_EducationCategory : BaseEntity2<Guid, Guid>
    {
        [ForeignKey("Id")]
        public virtual EducationCategory Category { get; set; }
        [ForeignKey("Id2")]
        public virtual Education Education { get; set; }
    }
}
