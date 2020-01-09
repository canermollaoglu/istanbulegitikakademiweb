using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("SuggestionByCategory")]
    public class Suggestion : BaseEntity<Guid>
    {
        public Suggestion()
        {
            Id = Guid.NewGuid();
        }
        public byte RangeMin { get; set; }
        public byte RangeMax { get; set; }

        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; }
        public virtual EducationCategory Category { get; set; }
    }
}
