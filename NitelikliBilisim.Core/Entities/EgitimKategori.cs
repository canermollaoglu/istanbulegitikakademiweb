using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    /// <summary>
    /// Id EgitimId
    /// Id2 KategoriId
    /// </summary>
    [Table("EgitimKategoriler")]
    public class EgitimKategori : BaseEntity2<Guid, Guid>
    {
        [ForeignKey(nameof(Id))]
        public Egitim Egitim { get; set; }
        [ForeignKey(nameof(Id2))]
        public Kategori Kategori { get; set; }
    }
}
