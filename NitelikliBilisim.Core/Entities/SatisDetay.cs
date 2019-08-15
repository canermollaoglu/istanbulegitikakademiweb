using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    /// <summary>
    /// Id satisId
    /// Id2 egitimId
    /// </summary>
    [Table("SatisDetaylar")]
    public class SatisDetay : BaseEntity2<Guid, Guid>
    {
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Fiyat { get; set; }
        [Range(0, 1), Column(TypeName = "decimal(3, 3)")]
        public decimal Indirim { get; set; }
        [ForeignKey(nameof(Id))]
        public Satis Satis { get; set; }
        [ForeignKey(nameof(Id2))]
        public Egitim Egitim { get; set; }
    }
}
