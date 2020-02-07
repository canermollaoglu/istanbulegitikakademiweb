using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Temp_UserSaleData")]
    public class TempSaleData
    {
        [Key, MaxLength(450)]
        public string Id { get; set; }
        [MaxLength(512)]
        public string Data { get; set; }
    }
}
