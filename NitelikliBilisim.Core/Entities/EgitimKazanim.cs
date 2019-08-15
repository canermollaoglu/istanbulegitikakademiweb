using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EgitimKazanimlar")]
    public class EgitimKazanim : BaseEntity<Guid>
    {
        public string KazanimAdi { get; set; }
        public string Simge { get; set; }
        public int Sira { get; set; }
        public Guid EgitimId { get; set; }


        [ForeignKey(nameof(EgitimId))]
        public Egitim Egitim { get; set; }
    }
}
