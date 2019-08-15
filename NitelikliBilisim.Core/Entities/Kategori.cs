using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Kategoriler")]
    public class Kategori : BaseEntity<Guid>
    {
        [Required, StringLength(150)]
        public string Ad { get; set; }
        [StringLength(500)]
        public string Aciklama { get; set; }
        [StringLength(250)]
        public string BannerFoto { get; set; }
        [StringLength(250)]
        public string DetayFoto { get; set; }
        [StringLength(250)]
        public string VideoUrl { get; set; }

        public ICollection<EgitimKategori> EgitimKategoriler { get; set; }
    }
}
