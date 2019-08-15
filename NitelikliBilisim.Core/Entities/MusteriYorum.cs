using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("MusteriYorumlar")]
    public class MusteriYorum : BaseEntity<Guid>
    {
        [Required, StringLength(500)]
        public string Yorum { get; set; }
        [Required, StringLength(128)]
        public string KullaniciId { get; set; }
        [Range(1, 5)]
        public int Puan { get; set; }
        [StringLength(128)]
        public string OnaylayanId { get; set; }
        public DateTime? OnaylanmaTarihi { get; set; }
        public Guid EgitimId { get; set; }


        [ForeignKey(nameof(EgitimId))]
        public Egitim Egitim { get; set; }
    }
}
