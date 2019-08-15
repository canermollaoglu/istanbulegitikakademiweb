using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Egitimler")]
    public class Egitim : BaseEntity<Guid>
    {
        [Required, StringLength(150)]
        public string Ad { get; set; }
        [Required]
        public string Aciklama { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Fiyat { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? EskiFiyat { get; set; }
        public int GunSayisi { get; set; }
        [StringLength(250)]
        public string EgitimFoto { get; set; }
        [StringLength(250)]
        public string DetayFoto { get; set; }
        [StringLength(250)]
        public string VideoUrl { get; set; }

        public ICollection<EgitimDetay> EgitimDetaylar { get; set; }
        public ICollection<MusteriYorum> MusteriYorumlar { get; set; }
        public ICollection<EgitimKazanim> EgitimKazanimlar { get; set; }
        public ICollection<EgitimKategori> EgitimKategoriler { get; set; }
    }
}
