using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums.educations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.educations
{
    [Table("EducationSuggestionCriterions")]
    public class EducationSuggestionCriterion : BaseEntity<Guid>
    {
        public EducationSuggestionCriterion()
        {
            Id = Guid.NewGuid();
        }

        public CriterionType CriterionType { get; set; }
        public int MinValue { get; set; }
        public int? MaxValue { get; set; }

        public Guid EducationId { get; set; }
        [ForeignKey("EducationId")]
        public virtual Education Education { get; set; }

    }
}
