using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("Addresses")]
    public class Address : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsDefaultAddress { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

    }
}
