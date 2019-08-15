using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Satislar")]
    public class Satis : BaseEntity<Guid>
    {
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string PostaKodu { get; set; }
        public FaturaTipi FaturaTipi { get; set; }
        public string FirmaAdi { get; set; }
        public string VergiNumarasi { get; set; }
        public string VergiDairesi { get; set; }
        public bool EFaturaMukellefiMi { get; set; }
        public string Adres { get; set; }
        [StringLength(16)]
        public string UserIp { get; set; }
        [StringLength(10)]
        public string KartBilgi { get; set; }
    }
}
