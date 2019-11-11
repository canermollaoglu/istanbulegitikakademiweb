using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Sales")]
    public class Sale : BaseEntity<Guid>
    {
        public Sale()
        {
            Id = Guid.NewGuid();
        }

        public CustomerType BillingType { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
    }
}
