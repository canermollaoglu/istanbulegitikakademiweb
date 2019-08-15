using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Sepetler")]
    public class Sepet : BaseEntity<Guid>
    {
        public Guid EgitimId { get; set; }
        public Guid? SatisId { get; set; }


        [ForeignKey(nameof(EgitimId))]
        public Egitim Egitim { get; set; }
        [ForeignKey(nameof(SatisId))]
        public Satis Satis { get; set; }
    }
}
