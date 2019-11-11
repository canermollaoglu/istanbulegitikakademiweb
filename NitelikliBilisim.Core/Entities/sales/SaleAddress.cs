using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    public class SaleAddress : BaseEntity<Guid>
    {
        [ForeignKey("Id")]
        public virtual Sale Sale { get; set; }
        
        [MaxLength(32)]
        public string City { get; set; }
        [MaxLength(32)]
        public string County { get; set; }
        [MaxLength(256)]
        public string Address { get; set; }
        [MaxLength(8)]
        public string PostalCode { get; set; }
    }
}
