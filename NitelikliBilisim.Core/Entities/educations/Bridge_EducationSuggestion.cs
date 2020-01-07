using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.educations
{
    [Table("Bridge_EducationSuggestion")]
    public class Bridge_EducationSuggestion : BaseEntity2<Guid, Guid>
    {
        [ForeignKey("Id")]
        public virtual Education Education { get; set; }
        [ForeignKey("Id2")]
        public virtual Suggestion Suggestion { get; set; }
    }
}
