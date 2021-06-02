using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationComponentItems")]
    public class EducationComponentItem: BaseEntity<Guid>
    {
        public EducationComponentItem()
        {
            Id = Guid.NewGuid();
        }

        public Guid EducationId { get; set; }
        public int Order { get; set; }
        public EducationComponentType ComponentType { get; set; }
        public EducationComponentSuggestionType SuggestionType { get; set; }

         [ForeignKey("EducationId")]
        public virtual Education Education { get; set; }
        
    }
}
