using NitelikliBilisim.Core.Abstracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Educators")]
    public class Educator : BaseEntity<string>
    {
        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
        [MaxLength(128)]
        public string Title { get; set; }
        [MaxLength(8192)]
        public string Biography { get; set; }
        /// <summary>
        /// Eğitmen Hakkında Kısa Bilgi
        /// </summary>
        [MaxLength(400)]
        public string ShortDescription { get; set; }

    }
}
