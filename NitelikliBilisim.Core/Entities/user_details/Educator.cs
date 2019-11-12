using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Educator")]
    public class Educator : BaseEntity<string>
    {
        [MaxLength(128)]
        public string Title { get; set; }
        [MaxLength(8192)]
        public string Biography { get; set; }
        [MaxLength(256)]
        public string PhotoUrl { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
    }
}
