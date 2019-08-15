using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EgitimDetaylari")]
    public class EgitimDetay : BaseEntity<Guid>
    {
        [Required, StringLength(120)]
        public string Baslik { get; set; }
        public string Konu { get; set; }
        public int Sira { get; set; }
        public Guid EgitimId { get; set; }


        [ForeignKey(nameof(EgitimId))]
        public Egitim Egitim { get; set; }
    }
}
