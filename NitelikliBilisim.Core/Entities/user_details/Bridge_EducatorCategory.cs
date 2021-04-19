using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("Bridge_EducatorCategories")]
    public class Bridge_EducatorCategory : BaseEntity2<string, Guid>
    {
        [ForeignKey("Id")]
        public virtual Educator Educator { get; set; }
        [ForeignKey("Id2")]
        public virtual EducationCategory EducationCategory { get; set; }
    }
}
