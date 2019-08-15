using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NitelikliBilisim.Core.Entities.Identity;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Egiticiler")]
    public class Egitici : BaseEntity<string>
    {
        [Required, StringLength(120)]
        public string Title { get; set; }
        public string Biyografi { get; set; }
        [StringLength(250)]
        public string DetayFoto { get; set; }

        public ApplicationUser Kullanici { get; set; }
    }
}
