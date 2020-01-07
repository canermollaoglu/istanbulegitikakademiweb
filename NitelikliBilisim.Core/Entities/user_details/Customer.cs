using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Customers")]
    public class Customer : BaseEntity<string>
    {
        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
        public CustomerType CustomerType { get; set; }
        public bool IsNbuyStudent { get; set; }
    }
}
